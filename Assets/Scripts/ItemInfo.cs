using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInfo : MonoBehaviour
{
    [Serializable]
    public struct Property
    {
        public TranslatableText title;
        [Range(1, 10)] public int value;
    }

    public new TranslatableText name;
    public TranslatableText description;
    public Property property1;
    public Property property2;
    public int cost;
}
