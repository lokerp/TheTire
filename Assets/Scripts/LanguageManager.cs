using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour, IDataControllable
{
    public static LanguageManager Instance { get; private set; }
    public Languages language;
    private Image iconHolder;
    private TextController[] controllers;
    public Slider? slider;
    public Sprite englishIcon;
    public Sprite russianIcon;

    public LanguageManager() { }

    void OnDisable()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(CheckValueChange);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        if (slider != null)
            iconHolder = slider.handleRect.GetComponent<Image>();

        controllers = FindObjectsOfType<TextController>(true);
    }

    public void SaveData(ref Database database)
    {
        database.currentLanguage = language;
    }

    public void LoadData(Database database)
    {
        language = database.currentLanguage;
        if (slider != null)
            switch (language)
            {
                case Languages.English:
                    iconHolder.overrideSprite = englishIcon;
                    slider.value = 0;
                    break;
                case Languages.Russian:
                    iconHolder.overrideSprite = russianIcon;
                    slider.value = 1;
                    break;
            }

        ChangeLanguage();
        if (slider != null)
            slider.onValueChanged.AddListener(CheckValueChange);
    }

    void CheckValueChange(float value)
    {
        switch (value)
        {
            case 0:
                language = Languages.English;
                iconHolder.overrideSprite = englishIcon;
                break;
            case 1:
                language = Languages.Russian;
                iconHolder.overrideSprite = russianIcon;
                break;
        }

        ChangeLanguage();
        DataManager.Instance.SaveGame();
    }

    void ChangeLanguage()
    {
        foreach(var contr in controllers)
            contr.RefreshText();
    }
}
