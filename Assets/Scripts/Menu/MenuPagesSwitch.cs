using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPagesSwitch : Ston<MenuPagesSwitch>, IAudioPlayable, IAchievementsControllable
{
    [field: SerializeField]
    public Notification AdErrorNotification { get; private set; }
    private Action _adErrorDelegate;

    public GameObject backButton;
    public List<MenuPage> pages;
    public PageTypes startPage = PageTypes.MainMenu;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }
    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }

    private MainMenuPage _mainMenu;
    bool _isGameLaunching = false;
    private LinkedList<PageTypes> _pageHistory;
    private int _launchesCount;

    protected override void Awake()
    {
        base.Awake();
        _pageHistory = new LinkedList<PageTypes>();
        _mainMenu = (MainMenuPage) pages.Find((x) => x.pageType == PageTypes.MainMenu);
        _adErrorDelegate = () => AdErrorNotification.Open(null, 4, null);
    }

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
        APIBridge.OnAdvertisementError += _adErrorDelegate;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
        APIBridge.OnAdvertisementError -= _adErrorDelegate;
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
            _isGameLaunching = true;
            LaunchesManager.Instance.OnGameStart();
            ScenesManager.Instance.SwitchScene("Game");
        }
        else
        {
            PlaySound(AudioSources[4]);
            StartCoroutine(_mainMenu.ShowError());
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

    private async void GetCriticAchievement()
    {
        bool hasRated = await APIBridge.Instance.HasRatedAsync();
        if (hasRated)
            OnAchievementProgressChanged.Invoke(new AchievementProgress(1, true), 12);
    }

    public void AfterDataLoaded(Database database) 
    {
        #if !UNITY_EDITOR
        if (!DataManager.IsFullyLaunched()
         && database.totalLaunchesCount % 4 == 0)
            APIBridge.Instance.ShowFullscreenAdv();
        if (!AchievementsManager.Instance.GetAchievementProgressById(12).isEarned)
            GetCriticAchievement();
        #endif
    }
}
