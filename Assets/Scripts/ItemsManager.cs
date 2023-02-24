using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ItemsManager : MonoBehaviour
{
    public static ItemsManager Instance { get; private set; }
    [SerializeField] List<ItemInfo> _items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    public ItemInfo GetItemByType(ItemTypes type)
    {
        return _items.Find((x) => x.itemType.Equals(type));
    }
}
