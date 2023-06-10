using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour, IDataControllable
{
    public static LanguageManager Instance { get; private set; }
    public Languages language;
    private TextController[] controllers;
    public TMP_Dropdown dropdown;

    public LanguageManager() { }

    void OnEnable()
    {
        if (dropdown != null)
            dropdown.onValueChanged.AddListener(CheckValueChange);
    }

    void OnDisable()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(CheckValueChange);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        controllers = FindObjectsOfType<TextController>(true);
    }

    public void SaveData(ref Database database)
    {
        database.settings.currentLanguage = language;
    }

    public void LoadData(Database database)
    {
        language = database.settings.currentLanguage;
        if (dropdown != null)
            switch (language)
            {
                case Languages.English:
                    dropdown.value = 0;
                    break;
                case Languages.Russian:
                    dropdown.value = 1;
                    break;
            }

        ChangeLanguage();
    }

    void CheckValueChange(int value)
    {
        switch (value)
        {
            case 0:
                dropdown.value = 0;
                language = Languages.English;
                break;
            case 1:
                dropdown.value = 1;
                language = Languages.Russian;
                break;
        }

        ChangeLanguage();
    }

    void ChangeLanguage()
    {
        foreach(var contr in controllers)
            contr.RefreshText();
    }
}
