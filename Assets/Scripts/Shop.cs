using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Shop : MonoBehaviour, IDataControllable
{
    public ItemTypes.Type itemType;
    public Transform itemPlaceHolder;

    public List<ItemInfo> catalogue;

    public GameObject leftArrowHolder;
    public GameObject rightArrowHolder;
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
        switch (gameObject.tag)
        {
            case "LeftArrow":
                ShowPrev();
                break;
            case "RightArrow":
                ShowNext();
                break;
            case "DropdownArrow":
                OpenInfoFrame();
                break;
            case "OptionButton":
                InteractWithItem();
                break;
        }
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
            Debug.LogError("Ошибка! Попытка купить уже купленный предмет");
            return;
        }
        int exCode = MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount - currentItem.cost);

        if (exCode == 0)
            _shopOption = ShopOptionButton.ShopOption.NoMoney;
        else if (exCode == 1)
            boughtItems.Add(currentItem);
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
    }

    void SpawnItem()
    {
        foreach(Transform obj in itemPlaceHolder)
            Destroy(obj.gameObject);
        Debug.Log(currentItem);

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

        if (currentItemIndex >= catalogue.Count - 1)
            rightArrowHolder.SetActive(false);
        else if (!rightArrowHolder.activeInHierarchy)
            rightArrowHolder.SetActive(true);

        if (currentItemIndex <= 0)
            leftArrowHolder.SetActive(false);
        else if (!leftArrowHolder.activeInHierarchy)
            leftArrowHolder.SetActive(true);
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
}
