using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LaunchesManager : StonUndestroyable<LaunchesManager>, IDataControllable, IAchievementsControllable
{
    public int LaunchesAmount { get; private set; }

    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }
    public static event Action OnLaunchesChanged;
    public static event Action OnLaunchesLoaded;

    public int MaxLaunches { get; private set; } = 10;
    public float TimeToRecoverInS { get; private set; } = 30;

    private int _adWatchedCount;
    private int _totalLaunchesCount;

    private void OnEnable()
    {
        APIBridge.OnAdvertisementClose += OnAdvertisementCloseHandler;
    }

    private void OnDisable()
    {
        APIBridge.OnAdvertisementClose -= OnAdvertisementCloseHandler;
    }

    public void OnAdvertisementCloseHandler(bool hasGotReward)
    {
        if (hasGotReward)
        {
            _adWatchedCount++;
            ChangeLaunchesAmount(10, true);
            GetSponsorAchievement(AchievementsManager.Instance.GetAchievementInfoById(10));
        }
    }

    public void LoadData(Database database)
    {
        var launchesEarnedFromLastSession = 0;
        if (DataManager.IsFirstVisitPerSession())
        {
            var timePassed = (System.DateTime.UtcNow - database.lastSession).TotalSeconds;
            launchesEarnedFromLastSession = Mathf.Clamp((int)(timePassed / TimeToRecoverInS), 0, MaxLaunches);
        }

        ChangeLaunchesAmount(database.currentLaunches + launchesEarnedFromLastSession, false);
        OnLaunchesLoaded?.Invoke();
        _adWatchedCount = database.adWatchedCount;
        _totalLaunchesCount = database.totalLaunchesCount;
    }

    public void SaveData(ref Database database)
    {
        database.adWatchedCount = _adWatchedCount;
        database.currentLaunches = LaunchesAmount;
        database.totalLaunchesCount = _totalLaunchesCount;
    }

    public bool CanPlay()
    {
        if (LaunchesAmount > 0)
            return true;
        return false;
    }

    public void OnGameStart()
    {
        _totalLaunchesCount++;
        ChangeLaunchesAmount(LaunchesAmount - 1, true);
    }

    public void ChangeLaunchesAmount(int newLaunchesAmount, bool withSaving)
    {
        LaunchesAmount = Mathf.Clamp(newLaunchesAmount, 0, MaxLaunches);
        if (withSaving)
            OnLaunchesChanged?.Invoke();
    }

    public void GetSponsorAchievement(AchievementInfo achievement)
    {
        var progress = new AchievementProgress(_adWatchedCount, achievement);
        OnAchievementProgressChanged.Invoke(progress, 10);
    }

    public void AfterDataLoaded(Database database) { }
}
