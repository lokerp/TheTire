using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Icons;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextController : MonoBehaviour
{
    public TranslatableText text;
    private TextMeshProUGUI textHandler;


    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChange += RefreshText;
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChange -= RefreshText;
    }

    private void Awake()
    {
        textHandler = GetComponent<TextMeshProUGUI>();
    }

    void RefreshText(Languages language)
    {
        textHandler.text = text.GetText(language);
    }
}
