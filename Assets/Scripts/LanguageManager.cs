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
    public Slider slider;
    public Sprite englishIcon;
    public Sprite russianIcon;
    public event Action<Languages> OnLanguageChange;

    public LanguageManager() { }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(CheckValueChange);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(CheckValueChange);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);

        iconHolder = slider.handleRect.GetComponent<Image>();
    }

    public void SaveData(ref Database database)
    {
        database.currentLanguage = language;
    }

    public void LoadData(Database database)
    {
        language = database.currentLanguage;
        switch (language)
        {
            case Languages.English:
                iconHolder.overrideSprite = englishIcon;
                slider.value = 0;
                break;
            case Languages.Russian:
                iconHolder.overrideSprite = englishIcon;
                slider.value = 1;
                break;
        }

        OnLanguageChange.Invoke(language);
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
        OnLanguageChange.Invoke(language);
        DataManager.Instance.SaveGame();
    }
}