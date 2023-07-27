using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementsManager : StonUndestroyable<AchievementsManager>, IDataControllable
{
    [SerializeField] private List<AchievementInfo> achievementsList;
    public Records Records { get; private set; }
    public static event Action<Dictionary<byte, AchievementProgress>> OnAchievementsUpdate;
    public static event Action<AchievementInfo> OnAchievementEarned;


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

    public void AfterDataLoaded(Database database) { }
}
