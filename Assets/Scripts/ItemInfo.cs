using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Info", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public TypesUtility.Item.Type itemType;
    public TypesUtility.Item.Tire.Type tireType;
    public TypesUtility.Item.Weapon.Type weaponType;

    [Serializable]
    public struct Property
    {
        public TranslatableText title;
        [Range(1, 10)] public int value;
    }

    public new TranslatableText name;
    public TranslatableText description;
    public List<Property> properties;
    public int cost;

    public string path;
}
