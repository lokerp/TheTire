using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ItemType
{
    Tire,
}

[RequireComponent(typeof(Animator))]
public class Shop : MonoBehaviour, IDataControllable
{
    public ItemType itemType;
    public Transform itemPlaceHolder;
    public ItemInfo itemsToBuy;

    private ItemInfo currentItem;
    private List<ItemInfo> boughtItems;

    public TextController itemName;
    public TextController itemDescription;
     
    public GameObject property1Frame;
    public GameObject property2Frame;



    private Animator _animator;

    private void OnEnable()
    {
        UIEvents.OnUIClick += CheckForClick;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= CheckForClick;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void CheckForClick(GameObject gameObject)
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

    public void SaveData(ref Database database)
    {
        switch (itemType)
        {
            case ItemType.Tire:
                database.currentTire = currentItem;
                database.availableTires = boughtItems;
                break;
        }
    }

    public void LoadData(Database database)
    {
        switch (itemType)
        {
            case ItemType.Tire:
                currentItem = database.currentTire;
                boughtItems = database.availableTires;
                break;
        }

        SpawnItem(currentItem.gameObject);
    }

    void SpawnItem(GameObject item)
    {
        foreach(GameObject obj in itemPlaceHolder)
            obj.SetActive(false);

        item.SetActive(true);
    }

    void RefreshInfo(ItemInfo item)
    {
        itemName.text = item.name;
        itemDescription.text = item.description;
    }
}
