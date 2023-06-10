using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextController : MonoBehaviour
{
    public TranslatableText text;
    private TextMeshProUGUI _textHandler;

    private void Awake()
    {
        _textHandler = GetComponent<TextMeshProUGUI>();
    }

    public void RefreshText()
    {
        Languages language = LanguageManager.Instance.language;

        if (_textHandler == null)
            _textHandler = GetComponent<TextMeshProUGUI>();

        switch (language)
        {
            case Languages.English:
                _textHandler.text = text.English;
                break;
            case Languages.Russian:
                _textHandler.text = text.Russian;
                break;
        }
    }
}
