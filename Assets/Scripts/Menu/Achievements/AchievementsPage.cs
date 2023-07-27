using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementsPage : MenuPage, IDataControllable
{
    [Space]
    public List<CanvasGroup> pages;

    [Space, Header("Stats")]
    public TextMeshProUGUI recordDistanceHolder;
    public TextMeshProUGUI achievementsEarnedCountHolder;

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
    public List<AchievementHolder> achievementHolders;
    public AchievementDescription achievementDescription;

    public GameObject leftArrow;
    public GameObject rightArrow;

    private int currentPageIndex;
    private int _playerRecord;


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
    }

    protected override void Awake()
    {
        base.Awake();

        currentPageIndex = 0;
        OpenPage();
    }

    private void Start()
    {
        achievementDescription.Close();
    }

    void UIClickHandler(GameObject gameObject)
    {
        AchievementHolder clickedAchievement = achievementHolders.Find((item) => 
                                               item.GetComponent<ButtonHolder>().button == gameObject);
        if (clickedAchievement != null)
            achievementDescription.Show(clickedAchievement.GetInfo());
        else
            achievementDescription.Close();

        if (gameObject == leftArrow.GetComponent<ButtonHolder>().button)
        {
            currentPageIndex--;
            OpenPage();
        }
        else if (gameObject == rightArrow.GetComponent<ButtonHolder>().button)
        {
            currentPageIndex++;
            OpenPage();
        }
    }

    void OpenPage()
    {
        bool hasOpened = false;

        for (int i = 0; i < pages.Count; i++)
        {
            if (i == currentPageIndex)
            {
                pages[i].alpha = 1;
                pages[i].blocksRaycasts = true;
                hasOpened = true;
            }
            else
            {
                pages[i].alpha = 0;
                pages[i].blocksRaycasts = false;
            }
        }

        if (currentPageIndex == 0) leftArrow.SetActive(false);
        else leftArrow.SetActive(true);
        if (currentPageIndex == pages.Count - 1) rightArrow.SetActive(false);
        else rightArrow.SetActive(true);

        if (!hasOpened)
            throw new System.Exception("Error! Achievements Page hasn't been opened!");
    }

    private void RefreshStats(int record, int achievementsEarnedCount)
    {
        recordDistanceHolder.text = record.ToString();
        achievementsEarnedCountHolder.text = achievementsEarnedCount + "/" + AchievementsManager.Instance.GetAchievementsList().Count;
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
        LeaderboardEntry playerEntry = default;
        bool wasPlayerEntryGot = false;
        try
        {
            leaderbord = await DataManager.Instance.GetLeaderboardAsync();
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error while loading leaderbord: {e}");
            errorText.SetActive(true);
            recordHoldersGrid.SetActive(false);
            return;
        }
        try
        {
            playerEntry = await DataManager.Instance.GetLeaderboardPlayerEntryAsync();
            wasPlayerEntryGot = true;
        }
        catch
        {
            wasPlayerEntryGot = false;
        }
        if (wasPlayerEntryGot)
        {
            int playerIndex = leaderbord.FindIndex((i) => i.uniqueID == playerEntry.uniqueID);
            if (playerIndex == -1)
                leaderbord[^1] = playerEntry;
        }
        int i;
        int currentEntriesCount = Mathf.Min(leaderbord.Count, recordHolders.Count);
        Task[] tasks = new Task[currentEntriesCount];
        for (i = 0; i < currentEntriesCount; i++)
            tasks[i] = SetPlayerRecordInfo(recordHolders[i], leaderbord[i], playerEntry.uniqueID);
        await Task.WhenAll(tasks);
        for (; i < recordHolders.Count; i++)
            recordHolders[i].SetRecordInfo(emptyText, defaultAvatar, i + 1, 0, otherPlayerColor);
    }

    private async Task SetPlayerRecordInfo(RecordHolder recordHolder, LeaderboardEntry entry, string playerID)
    {
        Texture2D avatar = defaultAvatar;
        TranslatableText name = noInfoName;
        int score = entry.score;
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
            score = _playerRecord;
        }

        recordHolder.SetRecordInfo(name, avatar, rank, score, nameColor);
    }

    public void SaveData(ref Database database) { }

    public void LoadData(Database database)
    {
        _playerRecord = (int) database.records.RecordDistance;
        RefreshRecords();
        RefreshStats((int) database.records.RecordDistance, database.records.AchievementsEarnedCount);
    }

    public void AfterDataLoaded(Database database) { }
}
