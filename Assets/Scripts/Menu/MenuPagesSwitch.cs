using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPagesSwitch : Ston<MenuPagesSwitch>, IAudioPlayable, IDataControllable, IAchievementsControllable
{
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
            LaunchesManager.Instance.ChangeLaunchesAmount(LaunchesManager.Instance.LaunchesAmount - 1, false);
            _isGameLaunching = true;
            _launchesCount++;
            ScenesManager.Instance.SwitchScene("Game");
        }
        else
        {
            PlaySound(AudioSources[4]);
            StopCoroutine(_mainMenu.ShowError());
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

    public void SaveData(ref Database database)
    {
        database.totalLaunchesCount = _launchesCount;
    }

    public void LoadData(Database database)
    {
        _launchesCount = database.totalLaunchesCount;
    }

    private async void GetCriticAchievement()
    {
        bool hasRated = await DataManager.Instance.HasRatedAsync();
        if (hasRated)
            OnAchievementProgressChanged.Invoke(new AchievementProgress(1, true), 12);
    }

    public void AfterDataLoaded(Database database) 
    {
        #if !UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "Menu"
         && database.totalLaunchesCount % 5 == 0 
         && database.totalLaunchesCount != 0)
            APIBridge.Instance.ShowFullscreenAdv();
        if (!AchievementsManager.Instance.GetAchievementProgressById(12).isEarned)
            GetCriticAchievement();
        #endif
    }
}
