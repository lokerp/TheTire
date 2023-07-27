using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : StonUndestroyable<LanguageManager>, IDataControllable
{
    public Languages CurrentLanguage { get; private set; }
    private TextController[] controllers;

    void OnEnable()
    {
        SettingsPage.OnSettingsChanged += ChangeLanguage;
    }

    void OnDisable()
    {
        SettingsPage.OnSettingsChanged -= ChangeLanguage;
    }

    public void SaveData(ref Database database) { }

    public void LoadData(Database database)
    {
        controllers = FindObjectsOfType<TextController>(true);
        ChangeLanguage(database.settings);
    }

    void ChangeLanguage(Settings settings)
    {
        CurrentLanguage = settings.language;
        RefreshTextControllers();
    }

    void RefreshTextControllers()
    {
        foreach (var contr in controllers)
            contr.RefreshText();
    }

    public void AfterDataLoaded(Database database) { }
}
