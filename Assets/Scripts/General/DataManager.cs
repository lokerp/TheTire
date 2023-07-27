using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Data;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Mathematics;

public struct DataLoadingInfo
{
    public Database data;
    public string loadingStatus;
}

public struct PlayerInfo
{
    public bool isLoaded;
    public string name;
    public Texture2D avatar;
}

public struct LeaderboardEntry
{
    public string uniqueID;
    public string avatar;
    public string name;
    public int score;
    public int rank;
}


public class DataManager : StonUndestroyable<DataManager>
{
    [SerializeField] private string _fileName;

    public Image loadingProgressCircle;

    [Header("Defaults"), SerializeField] private Database _database;
    private List<IDataControllable> dataControllableObjects;
    private FileDataHandler fileDataHandler;

    private Action<AchievementInfo> OnAchievementEarnedSaveHandler;
    private Action<int, int> OnLevelUpgradeHandler;

    private PlayerInfo _playerInfo;
    private bool _isFirstVisitPerSession;
    private bool _shouldSaveDataToCloud;
    private float _loadingProgress;
    private bool _gameIsFullyLaunched;

    protected override void Awake()
    {
        base.Awake();
        fileDataHandler = new(Application.persistentDataPath, _fileName);
    }

    private void Start()
    {
        InitGameAsync();
    }

    public async void InitGameAsync()
    {
        _shouldSaveDataToCloud = true;
        _loadingProgress = 0.1f;
        StartCoroutine(ControlLoadingScreen());

        OnAchievementEarnedSaveHandler = (_) => SaveGame();
        OnLevelUpgradeHandler = (_, _) => SaveGame();
        ScenesManager.OnSceneChanging += SaveGame;
        LaunchesManager.OnLaunchRestore += SaveGame;
        ShopPage.OnSelectedItemChanged += SaveGame;
        SettingsPage.OnSettingsSaved += SaveGame;
        UpgradesPage.OnLevelUp += OnLevelUpgradeHandler;
        AchievementsManager.OnAchievementEarned += OnAchievementEarnedSaveHandler;

        SceneManager.activeSceneChanged += (_, _) => LoadGame();
        SettingsPage.OnSettingsAuth += LoadGame;

        _loadingProgress = 0.5f;

        Database serverDatabase = await LoadDataFromServerAsync();
        if (serverDatabase != null)
        {
            fileDataHandler.Save(serverDatabase);
            _database = serverDatabase;
        }
        else
        {
            Database loadedDatabase = fileDataHandler.Load();
            if (loadedDatabase != null)
                _database = loadedDatabase;
            else
                CreateDatabase();
        }
        SetLeaderboardScore((int) _database.records.RecordDistance);

        _loadingProgress = 0.8f;

        _isFirstVisitPerSession = true;
        LoadGame();
        _isFirstVisitPerSession = false;
        _loadingProgress = 1;
    }


    public void SaveGame()
    {
        foreach (var obj in dataControllableObjects)
            obj.SaveData(ref _database);

        _database.lastSession = System.DateTime.UtcNow;
#if !UNITY_EDITOR
        if (_shouldSaveDataToCloud)
            APIBridge.Instance.SaveData(_database);
#endif
        fileDataHandler.Save(_database);

        UnityEngine.Debug.Log("Game saved");
    }

    public void LoadGame()
    {
        dataControllableObjects = FindAllDataControllables();
        UnityEngine.Debug.Log("Game loaded");
        foreach (var obj in dataControllableObjects)
            obj.LoadData(_database);
        foreach (var obj in dataControllableObjects)
            obj.AfterDataLoaded(_database);
#if !UNITY_EDITOR
        if (_gameIsFullyLaunched)
        {
            APIBridge.Instance.OnGameReady();
            _gameIsFullyLaunched = false;
        }
#endif
    }

    List<IDataControllable> FindAllDataControllables()
    {
        return FindObjectsOfType<MonoBehaviour>(true).OfType<IDataControllable>().ToList();
    }

#if UNITY_EDITOR
    public void OpenSavesFolder()
    {
        Process.Start(Application.persistentDataPath);
    }
#endif

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void CreateDatabase()
    {
        if (Application.systemLanguage == SystemLanguage.Russian
         || Application.systemLanguage == SystemLanguage.Belarusian
         || Application.systemLanguage == SystemLanguage.Ukrainian)
            _database.settings.language = Languages.Russian;
        else
            _database.settings.language = Languages.English;
        _database.records = new Records();
    }

    public static bool IsFirstVisitPerSession()
    {
        return Instance._isFirstVisitPerSession;
    }

    public IEnumerator ControlLoadingScreen()
    {
        float oldProgress = _loadingProgress;
        while (_loadingProgress < 1)
        {
            loadingProgressCircle.fillAmount = Mathf.SmoothStep(oldProgress, _loadingProgress, Time.deltaTime * 5);
            oldProgress = loadingProgressCircle.fillAmount;
            yield return null;
        }
        loadingProgressCircle.fillAmount = 1;
        yield return new WaitForSeconds(1f);
        _gameIsFullyLaunched = true;
        ScenesManager.Instance.SwitchScene("Menu");
    }

    public async Task<bool> HasRatedAsync()
    {
        return await APIBridge.Instance.HasRatedAsync();
    }

    public static bool IsPlayerAuth()
    {
#if UNITY_EDITOR
        return false;
#endif
        return APIBridge.Instance.IsPlayerAuth();
    }

    public bool HasPlayerPermission()
    {
#if UNITY_EDITOR
        return false;
#endif
        return APIBridge.Instance.HasPlayerPermission();
    }

    public async Task RequestPlayerPermissionAsync()
    {
        await APIBridge.Instance.RequestPlayerPermissionAsync();
    }

    public async Task<PlayerInfo> GetPlayerInfoAsync()
    {
        if (!_playerInfo.isLoaded)
            _playerInfo = await APIBridge.Instance.GetPlayerInfoAsync();
        return _playerInfo;
    }

    public async Task AuthAsync()
    {
#if UNITY_EDITOR
        return;
#endif
        if (!IsPlayerAuth())
        {
            await APIBridge.Instance.AuthAsync();
            if (IsPlayerAuth())
            {
                Database serverDatabase = await LoadDataFromServerAsync();
                UnityEngine.Debug.Log($"Loaded data: {serverDatabase}");
                if (serverDatabase != null)
                {
                    fileDataHandler.Save(serverDatabase);
                    _database = serverDatabase;
                    SetLeaderboardScore((int)serverDatabase.records.RecordDistance);
                }
            }
        }
    }

    public async Task<Database> LoadDataFromServerAsync()
    {
#if UNITY_EDITOR
        return null;
#endif
        _shouldSaveDataToCloud = true;
        DataLoadingInfo dataLoadingInfo = default;
        try
        {
            dataLoadingInfo = await APIBridge.Instance.LoadDataAsync();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex);
            _shouldSaveDataToCloud = false;
        }

        if (dataLoadingInfo.loadingStatus == "NOTAUTH" 
         || dataLoadingInfo.loadingStatus == "ERROR")
            _shouldSaveDataToCloud = false;

        return dataLoadingInfo.data;
    }


    public void SetLeaderboardScore(int score)
    {
#if !UNITY_EDITOR
        APIBridge.Instance.SetLeaderboardScore(score);
#endif
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
    {
        return await APIBridge.Instance.GetLeaderboardAsync();
    }

    public async Task<LeaderboardEntry> GetLeaderboardPlayerEntryAsync()
    {
        return await APIBridge.Instance.GetLeaderboardPlayerEntryAsync();
    }

    public static async Task<Texture2D> LoadImageFromInternetAsync(string url)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);

        var asyncOp = uwr.SendWebRequest();
        while (!asyncOp.isDone)
            await Task.Yield();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            throw new DataLoadException(uwr.error);
        }
        else
            return DownloadHandlerTexture.GetContent(uwr);
    }
}