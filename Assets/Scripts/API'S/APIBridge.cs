using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DataLoadException : Exception
{
    public DataLoadException(string msg) : base(msg) { }
}

public class APIBridge : StonUndestroyable<APIBridge>
{
    public enum API
    {
        Yandex,
        VK
    }

    public API api;
    public static event Action OnAdvertisementOpen;
    public static event Action<bool> OnAdvertisementClose;

    private string _jsonData = null;
    private bool _wasDataLoadingFinished = false;
    private string _dataLoadingStatus;

    private bool _wasRatedLoadingFinished = false;
    private bool _hasRated;

    private bool _wasAuthLoadingFinished = false;

    private bool _playerPermission;

    private bool _wasPlayerPermissionRequestFinished = false;

    private bool _wasLeaderbordLoadingFinished = false;
    private string _jsonLeaderbord = null;

    private bool _wasLeaderbordPlayerEntryLoadingFinished = false;
    private string _jsonLeaderbordPlayerEntry = null;

    public void SaveData(Database data)
    {
        switch (api)
        {
            case API.Yandex:
                ExternAPI.SaveData(JsonConvert.SerializeObject(data, Formatting.Indented));
                break;
            case API.VK:
                break;
        }
    }

    public async Task<DataLoadingInfo> LoadDataAsync()
    {
        _wasDataLoadingFinished = false;
        Database data = null;

        switch (api)
        {
            case API.Yandex:
                ExternAPI.LoadData();
                break;
            case API.VK:
                break;
        }

        while (true)
        {
            if (_wasDataLoadingFinished)
                break;
            await Task.Yield();
        }

        if (string.IsNullOrEmpty(_dataLoadingStatus))
            throw new DataLoadException($"Error while getting data loading status");

        if (_dataLoadingStatus == "SUCCESS")
            data = JsonConvert.DeserializeObject<Database>(_jsonData);

        return new DataLoadingInfo { data = data, loadingStatus = _dataLoadingStatus };
    }

    public void ShowRewardedAdv()
    {
        switch (api)
        {
            case API.Yandex:
                ExternAPI.ShowRewardedAdv();
                break;
        }
    }

    public void ShowFullscreenAdv()
    {
        switch (api)
        {
            case API.Yandex:
                ExternAPI.ShowFullscreenAdv();
                break;
        }
    }

    public async Task<bool> HasRatedAsync()
    {
        _wasRatedLoadingFinished = false;
        switch (api)
        {
            case API.Yandex:
                ExternAPI.HasRated();
                break;
        }

        while (true)
        {
            if (_wasRatedLoadingFinished)
                break;
            await Task.Yield();
        }

        return _hasRated;
    }

    public void SetLeaderboardScore(int score)
    {
        switch (api)
        {
            case API.Yandex:
                ExternAPI.SetLeaderboardScore(score);
                break;
        }
    }

    public bool IsPlayerAuth()
    {
        switch (api)
        {
            case API.Yandex:
                return ExternAPI.IsPlayerAuth();
        }

        return false;
    }

    public async Task<PlayerInfo> GetPlayerInfoAsync()
    {
        if (!HasPlayerPermission())
            throw new DataLoadException("Error while loading player info, no permissions!");

        PlayerInfo playerInfo = new();
        string infoJson = null;
        string[] nameAndAvatar = null;

        switch (api)
        {
            case API.Yandex:
                infoJson = ExternAPI.GetPlayerInfo();
                break;
        }
        if (string.IsNullOrEmpty(infoJson))
            throw new DataLoadException("Error while loading player info, infoJson is null");
        nameAndAvatar = JsonConvert.DeserializeObject<string[]>(infoJson);
        if (nameAndAvatar == null 
            || string.IsNullOrEmpty(nameAndAvatar[0]) 
            || string.IsNullOrEmpty(nameAndAvatar[1]))
            throw new DataLoadException("Error while loading player info, nameAndAvatar is null");

        playerInfo.isLoaded = true;
        playerInfo.name = nameAndAvatar[0];
        playerInfo.avatar = await DataManager.LoadImageFromInternetAsync(nameAndAvatar[1]);
        return playerInfo;
    }

    public async Task AuthAsync()
    {
        _wasAuthLoadingFinished = false;
        switch (api)
        {
            case API.Yandex:
                ExternAPI.Auth();
                break;
        }

        while (true)
        {
            if (_wasAuthLoadingFinished)
                break;
            await Task.Yield();
        }
    }

    public bool HasPlayerPermission()
    {
        switch (api)
        {
            case API.Yandex:
                _playerPermission = ExternAPI.HasPlayerPermission();
                break;
        }

        return _playerPermission;

    }

    public async Task RequestPlayerPermissionAsync()
    {
        _wasPlayerPermissionRequestFinished = false;

        switch (api)
        {
            case API.Yandex:
                ExternAPI.RequestPlayerPermission();
                break;
        }

        while (true)
        {
            if (_wasPlayerPermissionRequestFinished)
                break;
            await Task.Yield();
        }
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
    {
        _wasLeaderbordLoadingFinished = false;

        switch (api)
        {
            case API.Yandex:
                ExternAPI.GetLeaderboard();
                break;
        }

        while (true)
        {
            if (_wasLeaderbordLoadingFinished)
                break;
            await Task.Yield();
        }

        return JsonConvert.DeserializeObject<List<LeaderboardEntry>>(_jsonLeaderbord);
    }

    public async Task<LeaderboardEntry> GetLeaderboardPlayerEntryAsync()
    {
        _wasLeaderbordPlayerEntryLoadingFinished = false;

        switch (api)
        {
            case API.Yandex:
                ExternAPI.GetLeaderboardPlayerEntry();
                break;
        }

        while (true)
        {
            if (_wasLeaderbordPlayerEntryLoadingFinished)
                break;
            await Task.Yield();
        }

        return JsonConvert.DeserializeObject<LeaderboardEntry>(_jsonLeaderbordPlayerEntry);
    }

    public void OnGameReady()
    {
        switch (api)
        {
            case API.Yandex:
                ExternAPI.OnGameReady();
                break;
            case API.VK:
                break;
        }
    }

    public void RaiseOnAdvertisementOpenEvent()
    {
        OnAdvertisementOpen?.Invoke();
    }

    public void RaiseOnAdvertisementCloseEvent(int hasGotReward)
    {
        OnAdvertisementClose?.Invoke(Convert.ToBoolean(hasGotReward));
    }

    public void LoadedDataCallback(string jsonData)
    {
        _jsonData = jsonData;
        _wasDataLoadingFinished = true;
    }

    public void LoadingStatusCallback(string status)
    {
        _dataLoadingStatus = status;
    }

    public void AuthCallback()
    {
        _wasAuthLoadingFinished = true;
    }

    public void HasRatedCallback(int hasRated)
    {
        _hasRated = Convert.ToBoolean(hasRated);
        _wasRatedLoadingFinished = true;
    }

    public void RequestPlayerPermissionCallback()
    {
        _wasPlayerPermissionRequestFinished = true;
    }

    public void GetLeaderbordCallback(string jsonData)
    {
        _jsonLeaderbord = jsonData;
        _wasLeaderbordLoadingFinished = true;
    }

    public void GetLeaderbordPlayerEntryCallback(string jsonData)
    {
        _jsonLeaderbordPlayerEntry = jsonData;
        _wasLeaderbordPlayerEntryLoadingFinished = true;
    }
}
