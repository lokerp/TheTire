using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementsManager : MonoBehaviour, IDataControllable
{
    public static AchievementsManager Instance { get; private set; }
    [SerializeField] private List<AchievementInfo> achievementsList;
    public Records Records { get; private set; }
    [SerializeField] private Notification _notification;
    [SerializeField] private float _notificationTimeOpenedInS;
    public static event Action<Dictionary<byte, AchievementProgress>> OnAchievementsLoad;
    public static event Action<AchievementInfo> OnAchievementEarned;

    void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        var achievementControllables = FindAllAchievementsControllables();
        foreach (var contr in achievementControllables)
            contr.OnAchievementProgressChanged += UpdateAchievementProgressById;
    }

    void IDataControllable.LoadData(Database database)
    {
        if (database.records == null)
        {
            Records = new Records();
            Records.AchievementProgress = InitAchievementProgress();
        }
        else
            Records = database.records;
        OnAchievementsLoad?.Invoke(Records.AchievementProgress);
    }

    void IDataControllable.SaveData(ref Database database)
    {
        database.records = Records;
    }

    private Dictionary<byte, AchievementProgress> InitAchievementProgress()
    {
        var achievementProgress = new Dictionary<byte, AchievementProgress>();
        foreach (var el in achievementsList)
        {
            if (achievementProgress.ContainsKey(el.id))
                throw new System.Exception($"Error! Key {el.id} of SO {el.name} already exists!");
            achievementProgress.Add(el.id, new AchievementProgress());
        }

        return achievementProgress;
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

    public void UpdateAchievementProgressById(AchievementProgress newProgress, byte id)
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
        }
    }
}
