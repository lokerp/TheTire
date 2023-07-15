using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MenuPagesSwitch : MonoBehaviour, IAudioPlayable, IDataControllable
{
    public int _launchesCount;
    public GameObject backButton;
    public List<MenuPage> pages;
    public PageTypes startPage = PageTypes.MainMenu;

    bool _isGameLaunching = false;
    private LinkedList<PageTypes> _pageHistory;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void Awake()
    {
        _pageHistory = new LinkedList<PageTypes>();
    }

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
    }

    private void Start()
    {
        OpenPage(startPage, true);
    }

    void UIClickHandler(GameObject gameObject)
    {
        switch (gameObject.tag)
        {
            case "StartButton":
                PlaySound(AudioSources[2]);
                if (!_isGameLaunching)
                    StartGame();
                break;
            case "WheelShop":
                OpenPage(PageTypes.WheelShop, true);
                PlaySound(AudioSources[0]);
                break;
            case "WeaponShop":
                OpenPage(PageTypes.WeaponShop, true);
                PlaySound(AudioSources[0]);
                break;
            case "Achievements":
                OpenPage(PageTypes.Achievements, true);
                PlaySound(AudioSources[0]);
                break;
            case "Settings":
                OpenPage(PageTypes.Settings, true);
                PlaySound(AudioSources[3]);
                break;
            case "Upgrades":
                OpenPage(PageTypes.Upgrades, true);
                PlaySound(AudioSources[3]);
                break;
            case "BackButton":
                if (_pageHistory.Count > 1)
                {
                    if (_pageHistory.Last.Value != PageTypes.Settings && _pageHistory.Last.Value != PageTypes.Upgrades)
                        PlaySound(AudioSources[1]);
                    OpenPage(_pageHistory.Last.Previous.Value, false);
                }
                break;
        }
    }

    private void StartGame()
    {
        if (LaunchesManager.Instance.CanPlay())
        {
            LaunchesManager.Instance.ReduceLaunchesAmount();
            _isGameLaunching = true;
            _launchesCount++;
            ScenesManager.Instance.SwitchScene("Game");
        }
        else
        {
            PlaySound(AudioSources[4]);
            StartCoroutine(LaunchesManager.Instance.ShowError());
        }
    }

    void OpenPage(PageTypes clickedType, bool addInHistory)
    {
        bool hasOpened = false;

        foreach (var page in pages)
        {
            if (page.pageType == clickedType)
            {
                page.Open();
                hasOpened = true;
                if (addInHistory)
                    _pageHistory.AddLast(page.pageType);
                else
                    _pageHistory.RemoveLast();
            }
            else
                page.Close();
        }

        if (_pageHistory.Count == 1)
            backButton.SetActive(false);
        else 
            backButton.SetActive(true);

        if (!hasOpened)
            throw new System.Exception("Ошибка. Страница не была открыта!");
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }

    public void SaveData(ref Database database)
    {
        database.launchesCount = _launchesCount;
    }

    public void LoadData(Database database)
    {
        _launchesCount = database.launchesCount;
    }
}
