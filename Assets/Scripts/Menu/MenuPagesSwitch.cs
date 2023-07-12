using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MenuPagesSwitch : MonoBehaviour, IAudioPlayable
{
    public GameObject backButton;
    public List<MenuPage> pages;
    public PageTypes startPage = PageTypes.MainMenu;

    private LinkedList<PageTypes> pageHistory;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void Awake()
    {
        pageHistory = new LinkedList<PageTypes>();
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
                if (pageHistory.Count > 1)
                {
                    if (pageHistory.Last.Value != PageTypes.Settings && pageHistory.Last.Value != PageTypes.Upgrades)
                        PlaySound(AudioSources[1]);
                    OpenPage(pageHistory.Last.Previous.Value, false);
                }
                break;
        }
    }

    private void StartGame()
    {
        if (LaunchesManager.Instance.CanPlay())
        {
            LaunchesManager.Instance.ReduceLaunchesAmount();
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
                    pageHistory.AddLast(page.pageType);
                else
                    pageHistory.RemoveLast();
            }
            else
                page.Close();
        }

        if (pageHistory.Count == 1)
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
}
