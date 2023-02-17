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
        UIEvents.Instance.OnUIClick += UIClickHandler;
    }

    private void OnDisable()
    {
        UIEvents.Instance.OnUIClick -= UIClickHandler;
    }

    void UIClickHandler(GameObject gameObject)
    {
        Debug.Log(gameObject.tag);
        switch (gameObject.tag)
        {
            case "WheelShop":
                OpenPage(PageTypes.WheelShop);
                currentPage = PageTypes.WheelShop;
                pageHistory.AddLast(currentPage);
                break;
            case "BackButton":
                if (pageHistory.Count > 1)
                {
                    OpenPage(pageHistory.Last.Previous.Value);
                    pageHistory.RemoveLast();
                }
                break;

        }
    }

    void OpenPage(PageTypes clickedType)
    {
        bool hasOpened = false;

        foreach (var page in pages)
        {
            if (page.pageType == clickedType)
            {
                page.Open();
                hasOpened = true;
            }
            else
                page.Close();
        }

        if (!hasOpened)
            throw new System.Exception("Ошибка. Страница не была открыта!");
    }
}
