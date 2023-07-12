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
    public TextMeshProUGUI costValueHolder;

    private TranslatableText _defaultItemDescription;
    private Animator _shopAnimator;
    private ShopOptionButton.ShopOption _shopOption;
    private ItemInfo selectedItem;
    private ItemInfo currentItem;
    private int currentItemIndex;
    private List<ItemInfo> boughtItems;

    private static int boughtItemsCount;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnUIClick += UIClickHandler;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnUIClick -= UIClickHandler;
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
        SortCatalogueByCost();
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
            case ShopOptionButton.ShopOption.Buy:
                BuyItem();
                break;
        }

        RefreshItemInfo(false);
    }


    void BuyItem()
    {
        if (boughtItems.Contains(currentItem)) 
        {
            Debug.LogError($"Error! You are trying to buy an item you already have {currentItem}");
            return;
        }
        int exCode = MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount - currentItem.cost);

        if (exCode == 0)
        {
            _shopOption = ShopOptionButton.ShopOption.NoMoney;
            PlaySound(AudioSources[1]);
        }
        else if (exCode == 1)
        {
            boughtItems.Add(currentItem);
            selectedItem = currentItem;
            boughtItemsCount++;
            if (boughtItemsCount == ItemsManager.Instance.GetItemsCount())
                GetCollectorAchievement();
            PlaySound(AudioSources[0]);
        }
    }

    public void SaveData(ref Database database)
    {
        switch (itemType)
        {
            case ItemTypes.Type.Tire:
                database.selectedTire = selectedItem.itemType;
                database.availableTires = boughtItems.Select(t => t.itemType).ToList();
                break;
            case ItemTypes.Type.Weapon:
                database.selectedWeapon = selectedItem.itemType;
                database.availableWeapons = boughtItems.Select(t => t.itemType).ToList();
                break;
        }
    }

    public void LoadData(Database database)
    {
        boughtItemsCount = database.availableTires.Count + database.availableWeapons.Count;
        switch (itemType)
        {
            case ItemTypes.Type.Tire:
                selectedItem = ItemsManager.Instance.GetItemByType(database.selectedTire);
                boughtItems = database.availableTires.Select(t => ItemsManager.Instance.GetItemByType(t)).ToList();
                break;
            case ItemTypes.Type.Weapon:
                selectedItem = ItemsManager.Instance.GetItemByType(database.selectedWeapon);
                boughtItems = database.availableWeapons.Select(t => ItemsManager.Instance.GetItemByType(t)).ToList();
                break;
        }

        currentItem = selectedItem;
        SwitchItem();
    }

    void SwitchItem()
    {
        GetCurrentItemIndex();
        SpawnItem();
        RefreshItemInfo(true);

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

    void RefreshItemInfo(bool isSwitching)
    {
        itemNameHolder.text = currentItem.name;
        itemNameHolder.RefreshText();

        itemDescriptionNameHolder.text = currentItem.name;
        itemDescriptionNameHolder.RefreshText();

        itemDescriptionText.text = _defaultItemDescription + currentItem.description;
        itemDescriptionText.RefreshText();

        costValueHolder.text = currentItem.cost.ToString();

        if (IsBought(currentItem))
            if (selectedItem == currentItem)
                _shopOption = ShopOptionButton.ShopOption.InUse;
            else
                _shopOption = ShopOptionButton.ShopOption.Use;
        else if (isSwitching)
            _shopOption = ShopOptionButton.ShopOption.Buy;
        optionButtonHolder.ChangeOption(_shopOption);
    }

    void GetCurrentItemIndex()
    {
        currentItemIndex = catalogue.IndexOf(currentItem);
    }

    void SortCatalogueByCost()
    {
        catalogue.Sort((x, y) => x.cost.CompareTo(y.cost));
    }

    bool IsBought(ItemInfo item)
    {
        if (boughtItems.Find((x) => x.name == item.name))
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
        OnAchievementProgressChanged.Invoke(progress, 5);
    }
}
