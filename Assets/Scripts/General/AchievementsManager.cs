using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementsManager : StonUndestroyable<AchievementsManager>, IDataControllable
{
    public float timeNotificationOpenedInS;
    public Notification notification;
    public AudioSource notificationSound;

    [SerializeField] private List<AchievementInfo> achievementsList;
    public Records Records { get; private set; }
    public static event Action<Dictionary<byte, AchievementProgress>> OnAchievementsUpdate;
    public static event Action<AchievementInfo> OnAchievementEarned;

    private void OnEnable()
    {
        OnAchievementEarned += OpenNotificationWithAchievement;
    }

    public void LoadData(Database database)
    {
        var achievementControllables = FindAllAchievementsControllables();
        foreach (var contr in achievementControllables)
            contr.OnAchievementProgressChanged += UpdateAchievementProgressById;
        Records = database.records;
        if (DataManager.IsFirstVisitPerSession())
            InitAchievementProgress();
        OnAchievementsUpdate?.Invoke(Records.AchievementProgress);
    }

    public void SaveData(ref Database database)
    {
        database.records = Records;
    }

    private void InitAchievementProgress()
    {
        achievementsList = achievementsList.Where(a => a.platform.HasFlag(APIBridge.Instance.Platform)).ToList();

        if (Records.AchievementProgress == null)
            Records.AchievementProgress = new();
        foreach (var el in achievementsList)
        {
            if (!Records.AchievementProgress.ContainsKey(el.id))
                Records.AchievementProgress.Add(el.id, new AchievementProgress());
        }
    }

    private List<IAchievementsControllable> FindAllAchievementsControllables()
    {
        return FindObjectsOfType<MonoBehaviour>(true).OfType<IAchievementsControllable>().ToList();
    }

    public AchievementInfo GetAchievementInfoById(byte id)
    {
        AchievementInfo foundEl = achievementsList.Find((it) => it.id == id);
        if (foundEl == null)
            Debug.Log("Error while getting achievement by ID, check ID's");
        return foundEl;
    }

    public List<AchievementInfo> GetAchievementsList() 
    {
        return achievementsList;
    }

    public AchievementProgress GetAchievementProgressById(byte id)
    {
        return Records.AchievementProgress[id];
    }

    private void UpdateAchievementProgressById(AchievementProgress newProgress, byte id)
    {
        if (!Records.AchievementProgress.ContainsKey(id))
            return;
        var oldProgress = Records.AchievementProgress[id];
        if (!oldProgress.isEarned && oldProgress.progress < newProgress.progress)
        {
            Records.AchievementProgress[id] = newProgress;
            if (newProgress.isEarned)
            {
                Records.AchievementsEarnedCount++;
                var achievement = GetAchievementInfoById(id);
                OnAchievementEarned?.Invoke(achievement);
            }
            OnAchievementsUpdate?.Invoke(Records.AchievementProgress);
        }
    }

    private void OpenNotificationWithAchievement(AchievementInfo achievement)
    {
        notification.Open((textControllers, imageControllers) =>
        {
            imageControllers["icon"].sprite = Resources.Load<Sprite>(achievement.imagePath);
            textControllers["reward"].Text = new TranslatableText { English = "+ " + achievement.moneyPrize.ToString(),
                                                                    Russian = "+ " + achievement.moneyPrize.ToString() };
            textControllers["description"].Text = achievement.title;
        }, timeNotificationOpenedInS, notificationSound);
    }

    public void AfterDataLoaded(Database database) { }
}
