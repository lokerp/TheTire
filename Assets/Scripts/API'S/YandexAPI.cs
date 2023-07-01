using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class YandexAPI
{
    [DllImport("__Internal")]
    public static extern void _SaveData(string jsonData);

    [DllImport("__Internal")]
    public static extern string _LoadData(ref bool hasEnded);

    [DllImport("__Internal")]
    public static extern void _ShowFullscreenAdv();

    [DllImport("__Internal")]
    public static extern void _ShowRewardedAdv();
}
