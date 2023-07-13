using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    [Serializable]
    public struct Requirements : IComparable<Requirements>
    {
        public int bouncinessLevel;
        public int powerLevel;

        public int CompareTo(Requirements other)
        {
            return (bouncinessLevel + powerLevel).CompareTo(other.bouncinessLevel + powerLevel);
        }
    }

    public ItemTypes itemType;

    public new TranslatableText name;
    public TranslatableText description;
    public Requirements requirements;

    public string path;
}
