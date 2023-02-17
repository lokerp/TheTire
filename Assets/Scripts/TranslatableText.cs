using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TranslatableText
{
    public string English;
    public string Russian;

    public string GetText(Languages language)
    {
        switch (language)
        {
            case Languages.English:
                return English;
            case Languages.Russian:
                return Russian;
            default:
                return English;
        }
    }
}