using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuPage : MenuPage
{
    [Header("Launches")]
    public TextMeshProUGUI launchesText;
    public Animator timePanel;
    public ButtonHolder timePanelButton;
    public TextMeshProUGUI timeText;
    public AcceptPage adAcceptPage;
    public Color launchesErrorColor;
    public Color launchesDefaultColor;

    private int _timePassedInS;
    private Action timePanelButtonOnClickHandler;

    [Space, Header("Money")]
    public TextMeshProUGUI moneyText;

    protected override void Awake()
    {
        base.Awake();

        timePanelButtonOnClickHandler = () =>
        {
            StopCoroutine(OpenTimePanel());
            StartCoroutine(OpenTimePanel());
        };
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        adAcceptPage.OnAccept += OnAcceptHandler;
        LaunchesManager.OnLaunchRestore += RefreshLaunchesTexts;
        LaunchesManager.OnLaunchesLoaded += RefreshLaunchesTexts;
        timePanelButton.OnClick += timePanelButtonOnClickHandler;
        MoneyManager.OnMoneyChanged += RefreshMoneyText;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        adAcceptPage.OnAccept -= OnAcceptHandler;
        LaunchesManager.OnLaunchRestore -= RefreshLaunchesTexts;
        LaunchesManager.OnLaunchesLoaded -= RefreshLaunchesTexts;
        timePanelButton.OnClick -= timePanelButtonOnClickHandler;
        MoneyManager.OnMoneyChanged -= RefreshMoneyText;
    }

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator OpenTimePanel()
    {
        if (!timePanel.GetBool("IsOpen"))
        {
            timePanel.SetBool("IsOpen", true);
            yield return new WaitForSecondsRealtime(3);
            timePanel.SetBool("IsOpen", false);
        }
    }

    public IEnumerator ShowError()
    {
        StartCoroutine(OpenTimePanel());

        for (int i = 0; i < 3; i++)
        {
            launchesText.color = launchesErrorColor;
            yield return new WaitForSecondsRealtime(0.2f);
            launchesText.color = launchesDefaultColor;
            yield return new WaitForSecondsRealtime(0.2f);
        }

        yield break;
    }

    public void OnAcceptHandler()
    {
        #if !UNITY_EDITOR
        if (LaunchesManager.Instance.LaunchesAmount < LaunchesManager.Instance.MaxLaunches)
            APIBridge.Instance.ShowRewardedAdv();
        #endif
        #if UNITY_EDITOR
        LaunchesManager.Instance.ChangeLaunchesAmount(10, true);
        RefreshLaunchesTexts();
        #endif
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            if (LaunchesManager.Instance.LaunchesAmount >= LaunchesManager.Instance.MaxLaunches)
            {
                yield return null;
                continue;
            }

            else if (_timePassedInS < LaunchesManager.Instance.TimeToRecoverInS)
            {
                _timePassedInS++;
            }

            else if (_timePassedInS >= LaunchesManager.Instance.TimeToRecoverInS)
            {
                _timePassedInS = 0;
                LaunchesManager.Instance.ChangeLaunchesAmount(LaunchesManager.Instance.LaunchesAmount + 1, true);
            }
            RefreshLaunchesTexts();
            yield return new WaitForSecondsRealtime(1);
        }
    }

    void RefreshLaunchesTexts()
    {
        launchesText.text = $"{LaunchesManager.Instance.LaunchesAmount}/{LaunchesManager.Instance.MaxLaunches}";
        
        if (LaunchesManager.Instance.LaunchesAmount == LaunchesManager.Instance.MaxLaunches)
        {
            timeText.text = $"0:00";
            return;
        }

        int minutes = (int)((LaunchesManager.Instance.TimeToRecoverInS - _timePassedInS) / 60);
        int seconds = (int)((LaunchesManager.Instance.TimeToRecoverInS - _timePassedInS) % 60);
        timeText.text = string.Format("{0}:{1:d2}", minutes, seconds);
    }

    void RefreshMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
}
