using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MenuPagesSwitch : MonoBehaviour
{
    public List<MenuPageHandler> pages;
    public PageTypes currentPage = PageTypes.MainMenu;
    private LinkedList<PageTypes> pageHistory;

    private void Awake()
    {
        pageHistory = new LinkedList<PageTypes>();
        pageHistory.AddFirst(currentPage);
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
        OpenPage(currentPage, false);
    }

    void UIClickHandler(GameObject gameObject)
    {
        Debug.Log(gameObject.tag);
        switch (gameObject.tag)
        {
            case "StartButton":
                StartGame();
                break;
            case "WheelShop":
                OpenPage(PageTypes.WheelShop, true);
                break;
            case "WeaponShop":
                OpenPage(PageTypes.WeaponShop, true);
                break;
            case "BackButton":
                if (pageHistory.Count > 1)
                {
                    OpenPage(pageHistory.Last.Previous.Value, false);
                    pageHistory.RemoveLast();
                }
                break;

        }
    }

    private void StartGame()
    {
        if (LaunchesManager.Instance.CanPlay())
        {
            ScenesManager.Instance.SwitchScene("Game");
        }
        else { }
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
                {
                    currentPage = page.pageType;
                    pageHistory.AddLast(page.pageType);
                }
            }
            else
                page.Close();
        }

        if (!hasOpened)
            throw new System.Exception("Ошибка. Страница не была открыта!");
    }
}
