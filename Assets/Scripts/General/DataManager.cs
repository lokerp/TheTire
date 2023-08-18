using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct DataLoadingInfo
{
    public Database data;
    public bool _shouldSaveDataToCloud;
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
    public long score;
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
    private Action<int> OnMoneyChangedHandler;

    private PlayerInfo _playerInfo;
    private DataLoadingInfo _dataLoadingInfo;
    private bool _isFirstVisitPerSession;
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
        _loadingProgress = 0.1f;
        StartCoroutine(ControlLoadingScreen());
        _dataLoadingInfo = default;

        OnAchievementEarnedSaveHandler = (_) => SaveGame();
        OnLevelUpgradeHandler = (_, _) => SaveGame();
        OnMoneyChangedHandler = (_) => SaveGame();
        LaunchesManager.OnLaunchesChanged += SaveGame;
        ShopPage.OnSelectedItemChanged += SaveGame;
        SettingsPage.OnSettingsSaved += SaveGame;
        MoneyManager.OnMoneyChanged += OnMoneyChangedHandler;
        AchievementsManager.OnAchievementEarned += OnAchievementEarnedSaveHandler;

        SceneManager.activeSceneChanged += (_, _) => LoadGame();
        SettingsPage.OnSettingsAuth += async () => { await OnAuth(); };

        _loadingProgress = 0.5f;

#if !UNITY_EDITOR
        await APIBridge.Instance.AuthAsync();    
        _dataLoadingInfo = await APIBridge.Instance.LoadDataAsync();
#endif
        if (_dataLoadingInfo.data != null)
        {
            fileDataHandler.Save(_dataLoadingInfo.data);
            _database = _dataLoadingInfo.data;
        }
        else
        {
            Database loadedDatabase = fileDataHandler.Load();
            if (loadedDatabase != null)
                _database = loadedDatabase;
            else
                CreateDatabase();
        }
        _loadingProgress = 0.8f;

        _isFirstVisitPerSession = true;
        LoadGame();
        _isFirstVisitPerSession = false;
        SaveGame();
        _loadingProgress = 1;
    }


    public void SaveGame()
    {
        foreach (var obj in dataControllableObjects)
            obj.SaveData(ref _database);

        _database.lastSession = System.DateTime.UtcNow;
#if !UNITY_EDITOR
        if (_dataLoadingInfo._shouldSaveDataToCloud) {
            APIBridge.Instance.SaveData(_database);
            APIBridge.Instance.SetLeaderboardScore((long) _database.records.RecordDistance);
        }
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

    public static bool IsFullyLaunched()
    {
        return Instance._gameIsFullyLaunched;
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

    public async Task<PlayerInfo> GetPlayerInfoAsync()
    {
        if (!_playerInfo.isLoaded)
            _playerInfo = await APIBridge.Instance.GetPlayerInfoAsync();
        return _playerInfo;
    }

    public async Task OnAuth()
    {
        _dataLoadingInfo = await APIBridge.Instance.LoadDataAsync();
        if (_dataLoadingInfo.data != null)
        {
            fileDataHandler.Save(_dataLoadingInfo.data);
            _database = _dataLoadingInfo.data;
            LoadGame();
        }
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