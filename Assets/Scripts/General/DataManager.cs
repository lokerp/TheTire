using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Data;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string _fileName;
    public static DataManager Instance { get; private set; }
    [Header("Defaults"), SerializeField] private Database _database;
    private List<IDataControllable> dataControllableObjects;
    private FileDataHandler fileDataHandler;
    private bool isFirstGameLaunch = false;

    private void OnEnable()
    {
        ScenesManager.OnSceneChanging += SaveGame;
    }

    private void OnDisable()
    {
        ScenesManager.OnSceneChanging -= SaveGame;
    }

    void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        fileDataHandler = new(Application.persistentDataPath, _fileName);
    }

    private void Start()
    {
        if (isFirstGameLaunch)
        {
            #if !UNITY_EDITOR
            Database serverDatabase = null;
            try { serverDatabase = APIBridge.Instance.LoadData(); }
            catch (System.Exception ex) { UnityEngine.Debug.Log(ex); }

            if (serverDatabase != null)
            {
                _database = serverDatabase;
                fileDataHandler.Save(_database);
            }
            #endif
        }
        dataControllableObjects = FindAllDataControllables();
        LoadGame();
    }

    public void SaveGame()
    {
        foreach (var obj in dataControllableObjects)
            obj.SaveData(ref _database);

        fileDataHandler.Save(_database);

        #if !UNITY_EDITOR
        APIBridge.SaveData(_database);
        #endif
    }

    public void LoadGame()
    {
        Database loadedDatabase = fileDataHandler.Load();
        if (loadedDatabase != null)
            _database = loadedDatabase;

        if (isFirstGameLaunch)
            _database.isFirstVisitPerSession = true;

        foreach (var obj in dataControllableObjects)
            obj.LoadData(_database);

        isFirstGameLaunch = false;
        _database.isFirstVisitPerSession = false;
    }

    List<IDataControllable> FindAllDataControllables()
    {
        return FindObjectsOfType<MonoBehaviour>(true).OfType<IDataControllable>().ToList();
    }

    public Database GetDataFromServer()
    {
        Database serverData = null;
        try
        {
            serverData = APIBridge.Instance.LoadData();
        }
        catch (System.Exception ex) { UnityEngine.Debug.Log(ex); }

        return serverData;
    }

    #if UNITY_EDITOR
    public void OpenSavesFolder()
    {
        Process.Start(Application.persistentDataPath);
    }
    #endif

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitialized()
    {
        Instance.isFirstGameLaunch = true;
    }

    private void OnApplicationQuit()
    {
        _database.lastSession = System.DateTime.UtcNow;
        SaveGame();
    }
}
