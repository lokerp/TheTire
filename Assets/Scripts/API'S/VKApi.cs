#if VK_BUILD
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class VKApi
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
    public static extern void SetLeaderboardScore(int distance);

    [DllImport("__Internal")]
    public static extern bool IsPlayerAuth();

    [DllImport("__Internal")]
    public static extern string GetPlayerInfo();

    [DllImport("__Internal")]
    public static extern void Auth();

    [DllImport("__Internal")]
    public static extern void OpenLeaderboard();
}
#endif
