using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Shop : MonoBehaviour, IDataControllable, IAudioPlayable
{
    public ItemTypes.Type itemType;
    public Transform itemPlaceHolder;

    public List<ItemInfo> catalogue;

    public GameObject leftArrowHolder;
    public GameObject rightArrowHolder;
    public GameObject dropdownArrowHolder;
    public TextController itemNameHolder;
    public TextController itemDescriptionHolder;
    public ShopOptionButton optionButtonHolder;
    public TextMeshProUGUI costValueHolder;

    public List<Property> propertyHolders;

    private Animator _animator;
    private ShopOptionButton.ShopOption _shopOption;
    private ItemInfo selectedItem;
    private ItemInfo currentItem;
    private int currentItemIndex;
    private List<ItemInfo> boughtItems;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        if (_animator.GetBool("IsInfoFrameOpen"))
            _animator.SetBool("IsInfoFrameOpen", false);
        else
            _animator.SetBool("IsInfoFrameOpen", true);
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
        RefreshItemInfo();
        DataManager.Instance.SaveGame();
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
        GetCurrentItemIndex();
        SwitchItem();
    }

    void SwitchItem()
    {
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

    void RefreshItemInfo()
    {
        if (propertyHolders.Count != currentItem.properties.Count)
            throw new System.Exception("Количество свойств меню != количеству свойств предмета!");

        itemNameHolder.text = currentItem.name;
        itemNameHolder.RefreshText();

        itemDescriptionHolder.text = currentItem.description;
        itemDescriptionHolder.RefreshText();

        costValueHolder.text = currentItem.cost.ToString();

        for (int i = 0; i < propertyHolders.Count; i++)
        {
            propertyHolders[i].title.text = currentItem.properties[i].title;
            propertyHolders[i].title.RefreshText();
            propertyHolders[i].rating.SetRating(currentItem.properties[i].value);
        }

        if (IsBought(currentItem))
            if (selectedItem == currentItem)
                _shopOption = ShopOptionButton.ShopOption.InUse;
            else
                _shopOption = ShopOptionButton.ShopOption.Use;
        else if (_shopOption != ShopOptionButton.ShopOption.NoMoney)
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
}
