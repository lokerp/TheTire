using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ShopPage : MenuPage, IDataControllable, IAudioPlayable, IAchievementsControllable
{
    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }

    [Space, Space]
    public ItemTypes.Type itemType;
    public Transform itemPlaceHolder;

    public List<ItemInfo> catalogue;

    public GameObject leftArrowHolder;
    public GameObject rightArrowHolder;
    public GameObject dropdownArrowHolder;
    public TextController itemNameHolder;
    public TextController itemDescriptionNameHolder;
    public TextController itemDescriptionText;
    public ShopOptionButton optionButtonHolder;
    public GameObject bouncinessReqHolder;
    public GameObject powerReqHolder;
    public TextMeshProUGUI bouncinessReqText;
    public TextMeshProUGUI powerReqText;

    public Color notEarnedReqColor;
    public Color earnedReqColor;

    private TranslatableText _defaultItemDescription;
    private Animator _shopAnimator;
    private ShopOptionButton.ShopOption _shopOption;
    private ItemInfo selectedItem;
    private ItemInfo currentItem;
    private int currentItemIndex;
    private List<ItemInfo> availableItems;
    private int currBouncinessLvl;
    private int currPowerLvl;

    private Action<int, int> OnLevelUpHandler;

    private static int availableItemsCount;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnLevelUpHandler = (bouncinessLvl, weaponLvl) =>
        {
            currBouncinessLvl = bouncinessLvl;
            currPowerLvl = weaponLvl;
            RefreshAvailableItems();
        };
        UIEvents.OnUIClick += UIClickHandler;
        UpgradesManager.OnLevelUp += OnLevelUpHandler;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnUIClick -= UIClickHandler;
        UpgradesManager.OnLevelUp -= OnLevelUpHandler;
    }

    public override void Close()
    {
        if (currentItem != selectedItem)
        {
            currentItem = selectedItem;
            SwitchItem();
        }
        base.Close();
    }

    private void Awake()
    {
        _shopAnimator = GetComponent<Animator>();
        _defaultItemDescription = itemDescriptionText.text;
    }

    private void Start()
    {
        SortCatalogue();
    }

    void UIClickHandler(GameObject gameObject)
    {
        if (gameObject == leftArrowHolder.GetComponent<ButtonHolder>().button)
            ShowPrev();
        else if (gameObject == rightArrowHolder.GetComponent<ButtonHolder>().button)
            ShowNext();
        else if (gameObject == dropdownArrowHolder.GetComponent<ButtonHolder>().button)
            OpenInfoFrame();
        else if (gameObject == optionButtonHolder.GetComponent<ButtonHolder>().button)
            InteractWithItem();
    }

    void ShowNext()
    {
        if (currentItemIndex < catalogue.Count - 1)
        {
            currentItem = catalogue[++currentItemIndex];
            SwitchItem();
        }
    }

    void ShowPrev()
    {
        if (currentItemIndex > 0)
        {
            currentItem = catalogue[--currentItemIndex];
            SwitchItem();
        }
    }

    void OpenInfoFrame()
    {
        if (_shopAnimator.GetBool("IsInfoFrameOpen"))
            _shopAnimator.SetBool("IsInfoFrameOpen", false);
        else
            _shopAnimator.SetBool("IsInfoFrameOpen", true);
    }

    void InteractWithItem()
    {
        switch (_shopOption)
        {
            case ShopOptionButton.ShopOption.Use:
                selectedItem = currentItem;
                PlaySound(AudioSources[0]);
                break;
            case ShopOptionButton.ShopOption.NotAvailable:
                PlaySound(AudioSources[1]);
                break;
        }

        RefreshItemInfo();
    }

    public void SaveData(ref Database database)
    {
        switch (itemType)
        {
            case ItemTypes.Type.Tire:
                database.selectedTire = selectedItem.itemType;
                database.availableTires = availableItems.Select(t => t.itemType).ToList();
                break;
            case ItemTypes.Type.Weapon:
                database.selectedWeapon = selectedItem.itemType;
                database.availableWeapons = availableItems.Select(t => t.itemType).ToList();
                break;
        }
    }

    public void LoadData(Database database)
    {
        availableItemsCount = database.availableTires.Count + database.availableWeapons.Count;
        currBouncinessLvl = database.bouncinessLevel;
        currPowerLvl = database.powerLevel;
        switch (itemType)
        {
            case ItemTypes.Type.Tire:
                selectedItem = ItemsManager.Instance.GetItemByType(database.selectedTire);
                availableItems = database.availableTires.Select(t => ItemsManager.Instance.GetItemByType(t)).ToList();
                break;
            case ItemTypes.Type.Weapon:
                selectedItem = ItemsManager.Instance.GetItemByType(database.selectedWeapon);
                availableItems = database.availableWeapons.Select(t => ItemsManager.Instance.GetItemByType(t)).ToList();
                break;
        }
        currentItem = selectedItem;

        RefreshAvailableItems();
        SwitchItem();
    }

    void SwitchItem()
    {
        GetCurrentItemIndex();
        SpawnItem();
        RefreshItemInfo();

        if (currentItemIndex >= catalogue.Count - 1) rightArrowHolder.SetActive(false);
        else rightArrowHolder.SetActive(true);
        if (currentItemIndex <= 0) leftArrowHolder.SetActive(false);
        else leftArrowHolder.SetActive(true);
    }

    void SpawnItem()
    {
        foreach(Transform obj in itemPlaceHolder)
            Destroy(obj.gameObject);
        GameObject spawnedObj = Instantiate(ItemsManager.PathToPrefab<GameObject>(currentItem.path), itemPlaceHolder, false);
        spawnedObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
                                                         | RigidbodyConstraints.FreezePositionZ
                                                         | RigidbodyConstraints.FreezeRotation;
    }

    void RefreshAvailableItems()
    {
        int newItemsCount = 0;
        foreach (var item in catalogue)
        {
            if (!availableItems.Contains(item) 
                && currBouncinessLvl >= item.requirements.bouncinessLevel 
                && currPowerLvl >= item.requirements.powerLevel)
                availableItems.Add(item);
        }
        availableItemsCount += newItemsCount;

        if (availableItemsCount == ItemsManager.Instance.GetItemsCount())
            GetCollectorAchievement();
    }

    void RefreshItemInfo()
    {
        itemNameHolder.text = currentItem.name;
        itemNameHolder.RefreshText();

        itemDescriptionNameHolder.text = currentItem.name;
        itemDescriptionNameHolder.RefreshText();

        itemDescriptionText.text = _defaultItemDescription + currentItem.description;
        itemDescriptionText.RefreshText();

        if (currentItem.requirements.bouncinessLevel > 0)
            bouncinessReqText.text = currentItem.requirements.bouncinessLevel.ToString();
        else bouncinessReqHolder.SetActive(false);
        if (currentItem.requirements.powerLevel > 0)
            powerReqText.text = currentItem.requirements.powerLevel.ToString();
        else powerReqHolder.SetActive(false);

        if (currentItem.requirements.bouncinessLevel > currBouncinessLvl)
            bouncinessReqText.color = notEarnedReqColor;
        else bouncinessReqText.color = earnedReqColor;
        if (currentItem.requirements.powerLevel > currPowerLvl)
            powerReqText.color = notEarnedReqColor;
        else powerReqText.color = earnedReqColor;

        if (IsAvailable(currentItem))
            if (selectedItem == currentItem)
                _shopOption = ShopOptionButton.ShopOption.InUse;
            else
                _shopOption = ShopOptionButton.ShopOption.Use;
        else
            _shopOption = ShopOptionButton.ShopOption.NotAvailable;
        optionButtonHolder.ChangeOption(_shopOption);
    }

    void GetCurrentItemIndex()
    {
        currentItemIndex = catalogue.IndexOf(currentItem);
    }

    void SortCatalogue()
    {
        catalogue = catalogue.OrderByDescending(x => IsAvailable(x)).ThenBy(x => x.requirements).ToList();
    }

    bool IsAvailable(ItemInfo item)
    {
        if (availableItems.Find((x) => x.name == item.name))
            return true;
        return false;
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }

    private void GetCollectorAchievement()
    {
        var progress = new AchievementProgress(1, true);
        OnAchievementProgressChanged?.Invoke(progress, 5);
    }
}
