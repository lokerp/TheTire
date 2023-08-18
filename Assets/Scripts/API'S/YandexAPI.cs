#if YANDEX_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class YandexAPI
{
    [DllImport("__Internal")]
    public static extern void SaveData(string jsonData);

    [DllImport("__Internal")]
    public static extern void LoadData();

    [DllImport("__Internal")]
    public static extern void ShowFullscreenAdv();

    [DllImport("__Internal")]
    public static extern void ShowRewardedAdv();

    [DllImport("__Internal")]
    public static extern bool HasRated();

    [DllImport("__Internal")]
    public static extern void SetLeaderboardScore(long distance);

    [DllImport("__Internal")]
    public static extern void GetLeaderboard();

    [DllImport("__Internal")]
    public static extern void GetLeaderboardPlayerEntry();

    [DllImport("__Internal")]
    public static extern bool IsPlayerAuth();

    [DllImport("__Internal")]
    public static extern string GetPlayerInfo();

    [DllImport("__Internal")]
    public static extern bool HasPlayerPermission();

    [DllImport("__Internal")]
    public static extern void OnGameReady();

    [DllImport("__Internal")]
    public static extern void RequestPlayerPermission();

    [DllImport("__Internal")]
    public static extern void Auth();
}
#endif
