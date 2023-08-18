using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class DashboardPage : MenuPage, IDataControllable
{
    [Space]
    public List<CanvasGroup> pages;

    [Space, Header("Stats")]
    public TextController recordDistanceHolder;
    public TextMeshProUGUI achievementsEarnedCountHolder;
    public TextController recordPlace;
    public TranslatableText unAuthPlaceText;
    public Color unAuthColor;

    [Space, Header("Records")]
    public GameObject errorText;
    public GameObject recordHoldersGrid;
    public List<RecordHolder> recordHolders;
    public TranslatableText noInfoName;
    public Texture2D defaultAvatar;
    public Color currentPlayerRecordColor;
    public TranslatableText currentPlayerRecordName;
    public Color otherPlayerColor;
    public TranslatableText emptyText;

    [Space, Header("Achievements")]
    public GameObject achievementPagePrefab;
    public AchievementDescription achievementDescription;
    public AudioSource achievementClickSound;
    private List<AchievementHolder> achievementHolders;

    public GameObject leftArrow;
    public GameObject rightArrow;

    private int _currentPageIndex;
    private float _playerRecord;
    private LeaderboardEntry _playerEntry;

    protected override void OnEnable()
    {
        base.OnEnable();

        UIEvents.OnUIClick += UIClickHandler;
        AchievementsManager.OnAchievementsUpdate += RefreshAchievements;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        UIEvents.OnUIClick -= UIClickHandler;
        AchievementsManager.OnAchievementsUpdate -= RefreshAchievements;
        achievementHolders.ForEach((h) => h.GetComponent<ButtonHolder>().OnClick -= OnButtonHolderClick);
    }

    protected override void Awake()
    {
        base.Awake();

        AddAchievementPages();

        _currentPageIndex = 0;
        OpenPage();
    }

    public override void Close()
    {
        base.Close();
        achievementDescription.Close();
    }

    private void AddAchievementPages()
    {
        const int achievementsPerPage = 6;
        int achievementsCount = AchievementsManager.Instance.GetAchievementsList().Count();
        int pagesCount = Mathf.CeilToInt((float) achievementsCount / achievementsPerPage);
        achievementHolders = new List<AchievementHolder>
        {
            Capacity = pagesCount * achievementsPerPage
        };

        for (int i = 0; i < pagesCount; i++)
        {
            CanvasGroup page = Instantiate(achievementPagePrefab, Canvas.transform).GetComponent<CanvasGroup>();
            page.alpha = 0;
            pages.Add(page);

            foreach (AchievementHolder holder in page.GetComponent<AchievementsPage>().Holders)
            {
                achievementHolders.Add(holder);
                var btnHolder = holder.GetComponent<ButtonHolder>();
                btnHolder.OnClick += OnButtonHolderClick;
                btnHolder.AudioSources[0] = achievementClickSound;
            }
        }
    }

    void OnButtonHolderClick(ButtonHolderClickEventArgs args)
    {
        var clickedAchievement = args.buttonHolder.GetComponent<AchievementHolder>().GetInfo();
        achievementDescription.Show(clickedAchievement);
    }

    void UIClickHandler(GameObject gameObject)
    {
        if (gameObject == leftArrow.GetComponent<ButtonHolder>().button)
        {
            _currentPageIndex--;
            OpenPage();
        }
        else if (gameObject == rightArrow.GetComponent<ButtonHolder>().button)
        {
            _currentPageIndex++;
            OpenPage();
        }
    }

    void OpenPage()
    {
        bool hasOpened = false;

        for (int i = 0; i < pages.Count; i++)
        {
            if (i == _currentPageIndex)
            {
                pages[i].alpha = 1;
                pages[i].blocksRaycasts = true;
                if (!pages[i].TryGetComponent<AchievementsPage>(out _))
                    achievementDescription.Close();
                hasOpened = true;
            }
            else
            {
                pages[i].alpha = 0;
                pages[i].blocksRaycasts = false;
            }
        }

        if (_currentPageIndex == 0) leftArrow.SetActive(false);
        else leftArrow.SetActive(true);
        if (_currentPageIndex == pages.Count - 1) rightArrow.SetActive(false);
        else rightArrow.SetActive(true);

        if (!hasOpened)
            throw new System.Exception("Error! Dashboard Page hasn't been opened!");
    }

    private void RefreshStats(int achievementsEarnedCount)
    {
        recordDistanceHolder.Text = MathfExtension.GetUnitsFromNumber(_playerRecord, false, false, true);
        achievementsEarnedCountHolder.text = achievementsEarnedCount + "/" + AchievementsManager.Instance.GetAchievementsList().Count;
        var recordPlaceTMP = recordPlace.GetComponent<TextMeshProUGUI>();
        recordPlaceTMP.color = otherPlayerColor;

        if (string.IsNullOrEmpty(_playerEntry.uniqueID))
        {
            recordPlace.Text = unAuthPlaceText;
            recordPlaceTMP.color = unAuthColor;
        }
        else 
        {
            recordPlace.Text = new() { English = _playerEntry.rank.ToString(),
                                       Russian = _playerEntry.rank.ToString()
            };
        }
    }

    public void RefreshAchievements(Dictionary<byte, AchievementProgress> achievementProgress)
    {
        achievementProgress = achievementProgress.OrderByDescending(x => x.Value.isEarned)
                                                 .ThenBy(x => AchievementsManager.Instance.GetAchievementInfoById(x.Key).isSecret)
                                                 .ToDictionary(pair => pair.Key, pair => pair.Value);
        if (achievementHolders.Count < achievementProgress.Count)
            throw new System.Exception("achievementHolders.Count < achievements.Count !!!");
        int i = 0;
        foreach (var el in achievementProgress)
        {
            if (i >= achievementHolders.Count)
                break;
            var achievement = AchievementsManager.Instance.GetAchievementInfoById(el.Key);
            var progress = el.Value;
            var canv = achievementHolders[i].GetComponent<CanvasGroup>();
            canv.alpha = 1;
            canv.blocksRaycasts = true;
            achievementHolders[i].SetInfo((achievement, progress));
            i++;
        }
        for (; i < achievementHolders.Count; i++)
        {
            var canv = achievementHolders[i].GetComponent<CanvasGroup>();
            canv.alpha = 0;
            canv.blocksRaycasts = false;
        }
    }

    public async Task RefreshRecords()
    {
        List<LeaderboardEntry> leaderbord = default;
        bool wasPlayerEntryGot = !string.IsNullOrEmpty(_playerEntry.uniqueID);
        try
        {
            leaderbord = await APIBridge.Instance.GetLeaderboardAsync();
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error while loading leaderbord: {e}");
            errorText.SetActive(true);
            recordHoldersGrid.SetActive(false);
            return;
        }

        int playerIndex = leaderbord.FindIndex((i) => i.uniqueID == _playerEntry.uniqueID);
        if (playerIndex == -1)
            leaderbord[^1] = _playerEntry;

        int i;
        int currentEntriesCount = Mathf.Min(leaderbord.Count, recordHolders.Count);
        Task[] tasks = new Task[currentEntriesCount];
        for (i = 0; i < currentEntriesCount; i++)
            tasks[i] = SetPlayerRecordInfo(recordHolders[i], leaderbord[i], _playerEntry.uniqueID);
        await Task.WhenAll(tasks);

        var zeroScore = MathfExtension.GetUnitsFromNumber(0, false, false, true);
        for (; i < recordHolders.Count; i++)
            recordHolders[i].SetRecordInfo(emptyText, defaultAvatar, i + 1, zeroScore, otherPlayerColor);
    }

    private async Task SetPlayerRecordInfo(RecordHolder recordHolder, LeaderboardEntry entry, string playerID)
    {
        Texture2D avatar = defaultAvatar;
        TranslatableText name = noInfoName;
        TranslatableText score = MathfExtension.GetUnitsFromNumber(entry.score, true, false, true);
        int rank = entry.rank;
        Color nameColor = otherPlayerColor;

        if (!string.IsNullOrEmpty(entry.name))
        {
            name = new TranslatableText { English = entry.name, Russian = entry.name };
            try { avatar = await DataManager.LoadImageFromInternetAsync(entry.avatar); }
            catch { }
        }

        if (entry.uniqueID == playerID)
        {
            name = currentPlayerRecordName;
            nameColor = currentPlayerRecordColor;
            score = MathfExtension.GetUnitsFromNumber(_playerRecord, true, false, true);
        }

        recordHolder.SetRecordInfo(name, avatar, rank, score, nameColor);
    }

    public void SaveData(ref Database database) { }

    public async void LoadData(Database database)
    {
        _playerRecord = database.records.RecordDistance;
        await GetPlayerEntryAsync();
        RefreshRecords();
        RefreshStats(database.records.AchievementsEarnedCount);
    }

    async Task GetPlayerEntryAsync()
    {
        try
        {
            _playerEntry = await APIBridge.Instance.GetLeaderboardPlayerEntryAsync();
            _playerEntry.score = (long) _playerRecord;
        }
        catch
        {
            _playerEntry = new()
            {
                uniqueID = "",
                avatar = "",
                name = "",
                score = (long)_playerRecord,
                rank = -1
            };
        }
    }

    public void AfterDataLoaded(Database database) { }
}
