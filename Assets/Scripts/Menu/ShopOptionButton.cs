using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopOptionButton : MonoBehaviour
{
    public enum ShopOption
    {
        NotAvailable,
        Use,
        InUse
    }

    public Color buyColor;
    public Color notAvailableColor;
    public Color useColor;
    public Color inUseColor;

    public TranslatableText notAvailableText;
    public TranslatableText useText;
    public TranslatableText inUseText;

    public TextMeshProUGUI _colorHolder;
    public TextController _textHolder;

    public void ChangeOption(ShopOption option)
    {
        switch (option)
        {
            case ShopOption.NotAvailable:
                _textHolder.text = notAvailableText;
                _colorHolder.color = notAvailableColor;
                break;
            case ShopOption.Use:
                _textHolder.text = useText;
                _colorHolder.color = useColor;
                break;
            case ShopOption.InUse:
                _textHolder.text = inUseText;
                _colorHolder.color = inUseColor;
                break;
        }

        _textHolder.RefreshText();
    }
}
