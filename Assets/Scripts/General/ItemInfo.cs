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
        public int adsViewed;

        public int CompareTo(Requirements other)
        {
            int compRes = 0;
            if (adsViewed > 0 || other.adsViewed > 0)
                compRes = adsViewed.CompareTo(other.adsViewed);
            else
                compRes = (bouncinessLevel + powerLevel).CompareTo(other.bouncinessLevel + other.powerLevel);

            return compRes;
        }
    }

    public ItemTypes itemType;

    public new TranslatableText name;
    public TranslatableText description;
    public Requirements requirements;

    public string path;
}
