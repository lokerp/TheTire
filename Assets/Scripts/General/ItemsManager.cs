using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ItemsManager : StonUndestroyable<ItemsManager>
{
    [SerializeField] List<ItemInfo> _items;

    public ItemInfo GetItemByType(ItemTypes type)
    {
        return _items.Find((x) => x.itemType.Equals(type));
    }

    public int GetItemsCount()
    {
        return _items.Count;
    }

    public static T PathToPrefab<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
}
