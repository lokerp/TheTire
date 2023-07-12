using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TranslatableText
{
    public string English;
    public string Russian;

    public static TranslatableText operator +(TranslatableText text1, TranslatableText text2)
    {
        TranslatableText newText = new()
        {
            Russian = text1.Russian + text2.Russian,
            English = text1.English + text2.English
        };
        return newText;
    }
}