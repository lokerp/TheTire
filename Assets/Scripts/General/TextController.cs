using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextController : MonoBehaviour
{
    public TranslatableText Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
            RefreshText();
        }
    }

    [SerializeField] private TranslatableText _text;
    private TextMeshProUGUI _textHandler;

    private void Awake()
    {
        _textHandler = GetComponent<TextMeshProUGUI>();
    }

    public void RefreshText()
    {
        var language = LanguageManager.Instance.CurrentLanguage;
        if (_textHandler == null)
            _textHandler = GetComponent<TextMeshProUGUI>();

        switch (language)
        {
            case Languages.English:
                _textHandler.text = Text.English;
                break;
            case Languages.Russian:
                _textHandler.text = Text.Russian;
                break;
        }
    }
}
