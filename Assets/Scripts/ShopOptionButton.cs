using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopOptionButton : MonoBehaviour
{
    public enum ShopOption
    {
        Buy,
        NoMoney,
        Use,
        InUse
    }

    public Color buyColor;
    public Color noMoneyColor;
    public Color useColor;
    public Color inUseColor;

    public TranslatableText buyText;
    public TranslatableText noMoneyText;
    public TranslatableText useText;
    public TranslatableText inUseText;

    public TextMeshProUGUI _colorHolder;
    public TextController _textHolder;

    public void ChangeOption(ShopOption option)
    {
        switch (option)
        {
            case ShopOption.Buy:
                _textHolder.text = buyText;
                _colorHolder.color = buyColor;
                break;
            case ShopOption.NoMoney:
                _textHolder.text = noMoneyText;
                _colorHolder.color = noMoneyColor;
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
