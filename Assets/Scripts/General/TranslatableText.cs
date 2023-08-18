using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public TranslatableText Concatenate(string text)
    {
        English ??= "";
        Russian ??= "";

        English += text;
        Russian += text;
        return this;
    }

    public static explicit operator TranslatableText(string str)
    {
        TranslatableText newText = new()
        {
            Russian = str,
            English = str
        };
        return newText;
    }

    public string GetText(Languages language)
    {
        return language switch
        {
            Languages.English => English,
            Languages.Russian => Russian,
            _ => throw new NotImplementedException(),
        };
    }
}