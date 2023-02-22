using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum ShopOption
{
    Buy,
    NoMoney,
    Use,
    InUse
}

[RequireComponent(typeof(Animator))]
public class Shop : MonoBehaviour, IDataControllable
{
    public TypesUtility.Item.Type itemType;
    public Transform itemPlaceHolder;
    public List<ItemInfo> catalogue;

    public TextController itemNameHolder;
    public TextController itemDescriptionHolder;

    public ShopOptionButton optionButtonHolder;

    public TextMeshProUGUI costValueHolder;
 
    public List<Property> propertyHolders;

    private Animator _animator;
    private ShopOption _shopOption;
    private ItemInfo currentItem;
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

    }

    void ShowPrev()
    {

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
        
    }

    public void SaveData(ref Database database)
    {
        switch (itemType)
        {
            case TypesUtility.Item.Type.Tire:
                break;
        }
    }

    public void LoadData(Database database)
    {
        switch (itemType)
        {
            case TypesUtility.Item.Type.Tire:
                break;
        }

        Debug.Log(currentItem);
        SpawnItem(currentItem);
    }

    public void SpawnItem(ItemInfo item)
    {
        foreach(GameObject obj in itemPlaceHolder)
            Destroy(obj);

        GameObject spawnedObj = Instantiate(SaveUtility.PathToPrefab<GameObject>(item.path), itemPlaceHolder, true);
        spawnedObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
                                                         | RigidbodyConstraints.FreezePositionZ
                                                         | RigidbodyConstraints.FreezeRotation;
        RefreshItemInfo(item);
    }

    void RefreshItemInfo(ItemInfo item)
    {
        if (propertyHolders.Count != item.properties.Count)
            throw new System.Exception("Количество свойств меню != количеству свойств предмета!");

        itemNameHolder.text = item.name;
        itemNameHolder.RefreshText();

        itemDescriptionHolder.text = item.description;
        itemDescriptionHolder.RefreshText();

        costValueHolder.text = item.cost.ToString();

        for (int i = 0; i < propertyHolders.Count; i++)
        {
            propertyHolders[i].title.text = item.properties[i].title;
            propertyHolders[i].title.RefreshText();
            propertyHolders[i].rating.SetRating(item.properties[i].value);
        }

        if (isBought(item))
            if (currentItem == item)
                _shopOption = ShopOption.InUse;
            else
                _shopOption = ShopOption.Use;
        else
            _shopOption = ShopOption.Buy;

        optionButtonHolder.ChangeOption(_shopOption);
    }

    void SortCatalogueByCost()
    {
        catalogue.Sort((x, y) => x.cost.CompareTo(y.cost));
    }

    bool isBought(ItemInfo item)
    {
        if (boughtItems.Find((x) => x.name == item.name))
            return true;
        return false;
    }
}
