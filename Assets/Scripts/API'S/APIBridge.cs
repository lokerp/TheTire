using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class DataLoadException : Exception
{
    public DataLoadException(string msg) : base(msg) { }
}

public class APIBridge : StonUndestroyable<APIBridge>
{
    [field: SerializeField]
    public Platforms Platform { get; private set; }

    public static event Action OnAdvertisementOpen;
    public static event Action<bool> OnAdvertisementClose;
    public static event Action OnAdvertisementError;

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
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        try
        {
            string encryptedJsonData = StringEncryptor.EncryptString(jsonData);
#if YANDEX_BUILD
            YandexAPI.SaveData(encryptedJsonData);
#endif
#if VK_BUILD
            VKApi.SaveData(JsonConvert.SerializeObject(data, Formatting.Indented));
#endif
        }
        catch { }
    }

    public async Task<DataLoadingInfo> LoadDataAsync()
    {
        _wasDataLoadingFinished = false;
        DataLoadingInfo dataLoadingInfo = new() { data = null, _shouldSaveDataToCloud = true };
#if YANDEX_BUILD
        YandexAPI.LoadData();
#endif
#if VK_BUILD
        VKApi.LoadData();
#endif

        while (true)
        {
            if (_wasDataLoadingFinished)
                break;
            await Task.Yield();
        }

        if (string.IsNullOrEmpty(_dataLoadingStatus))
        {
            Console.WriteLine("Error while getting data loading status");
            dataLoadingInfo._shouldSaveDataToCloud = false;
        }

        if (_dataLoadingStatus == "NOTAUTH"
         || _dataLoadingStatus == "ERROR")
            dataLoadingInfo._shouldSaveDataToCloud = false;
        else if (_dataLoadingStatus == "EMPTY")
            dataLoadingInfo._shouldSaveDataToCloud = true;

        if (_dataLoadingStatus == "OLD")
        {
            try
            {
                dataLoadingInfo.data = JsonConvert.DeserializeObject<Database>(_jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while loading old data: {ex}");
            }
        }

        else if (_dataLoadingStatus == "SUCCESS")
        {
            try
            {
                #if YANDEX_BUILD
                dataLoadingInfo.data = JsonConvert.DeserializeObject<Database>(StringEncryptor.DecryptString(_jsonData));
#endif
                #if VK_BUILD
                dataLoadingInfo.data = JsonConvert.DeserializeObject<Database>(_jsonData);
                #endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while loading new data: {ex}");
            }
        }
        Debug.Log($"Loaded data: {dataLoadingInfo.data}. Loading status: {_dataLoadingStatus}");
        return dataLoadingInfo;
    }

    public void ShowRewardedAdv()
    {
#if UNITY_EDITOR
        RaiseOnAdvertisementCloseEvent(1);
#endif
#if YANDEX_BUILD
        YandexAPI.ShowRewardedAdv();
#endif
#if VK_BUILD
        VKApi.ShowRewardedAdv();
#endif
    }

    public void ShowFullscreenAdv()
    {
#if YANDEX_BUILD
        YandexAPI.ShowFullscreenAdv();
#endif
#if VK_BUILD
        VKApi.ShowFullscreenAdv();
#endif
    }

    public async Task<bool> HasRatedAsync()
    {
        _wasRatedLoadingFinished = false;

#if YANDEX_BUILD
        YandexAPI.HasRated();
#endif
#if VK_BUILD
        return false;
#endif

        while (true)
        {
            if (_wasRatedLoadingFinished)
                break;
            await Task.Yield();
        }

        return _hasRated;
    }

    public void SetLeaderboardScore(long score)
    {
#if YANDEX_BUILD
        YandexAPI.SetLeaderboardScore(score);
#endif
#if VK_BUILD
        VKApi.SetLeaderboardScore(score);
#endif
    }

    public static bool IsPlayerAuth()
    {
#if YANDEX_BUILD
        return YandexAPI.IsPlayerAuth();
#endif
#if VK_BUILD
        return true;
#endif
        return false;
    }

    public async Task<PlayerInfo> GetPlayerInfoAsync()
    {
        if (!HasPlayerPermission())
            throw new DataLoadException("Error while loading player info, no permissions!");

        PlayerInfo playerInfo = new();
        string infoJson = null;
        string[] nameAndAvatar = null;

#if YANDEX_BUILD
        infoJson = YandexAPI.GetPlayerInfo();
#endif
#if VK_BUILD
        infoJson = VKApi.GetPlayerInfo();
#endif

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

#if YANDEX_BUILD
        YandexAPI.Auth();
#endif
#if VK_BUILD
        VKApi.Auth();
#endif

        while (true)
        {
            if (_wasAuthLoadingFinished)
                break;
            await Task.Yield();
        }
    }

    public bool HasPlayerPermission()
    {

#if YANDEX_BUILD
        _playerPermission = YandexAPI.HasPlayerPermission();
#endif
#if VK_BUILD
        return true;
#endif

        return _playerPermission;

    }

    public async Task RequestPlayerPermissionAsync()
    {
        _wasPlayerPermissionRequestFinished = false;

#if YANDEX_BUILD
        YandexAPI.RequestPlayerPermission();
#endif
#if VK_BUILD
        _wasPlayerPermissionRequestFinished = true;
#endif

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

#if YANDEX_BUILD
        YandexAPI.GetLeaderboard();
#endif
#if VK_BUILD || Server
        ServerAPI.GetLeaderboard();
#endif

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

#if YANDEX_BUILD
        YandexAPI.GetLeaderboardPlayerEntry();
#endif
#if VK_BUILD || Server
        ServerAPI.GetLeaderboardPlayerEntry();
#endif

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
#if YANDEX_BUILD
        YandexAPI.OnGameReady();
#endif
#if VK_BUILD

#endif
    }

    public void YandexCheatingCheckCallback(int record)
    {
        if ((int) AchievementsManager.Instance.Records.RecordDistance != record)
            SetLeaderboardScore((int)AchievementsManager.Instance.Records.RecordDistance);
    }

    public void RaiseOnAdvertisementOpenEvent()
    {
        OnAdvertisementOpen?.Invoke();
    }

    public void RaiseOnAdvertisementCloseEvent(int hasGotReward)
    {
        OnAdvertisementClose?.Invoke(Convert.ToBoolean(hasGotReward));
    }

    public void RaiseOnAdvertisementErrorEvent()
    {
        OnAdvertisementError?.Invoke();
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
