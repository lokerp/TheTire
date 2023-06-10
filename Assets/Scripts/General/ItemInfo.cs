using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public ItemTypes itemType;

    [Serializable]
    public struct Property
    {
        public TranslatableText title;
        [Range(0, 10)] public int value;
    }

    public new TranslatableText name;
    public TranslatableText description;
    public List<Property> properties;
    public int cost;

    public string path;
}
