using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string _fileName;
    public static DataManager Instance { get; private set; }
    [Header("Defaults"), SerializeField] private Database _database;
    private List<IDataControllable> dataControllableObjects;
    private FileDataHandler fileDataHandler;

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
    }

    private void Start()
    {
        fileDataHandler = new(Application.persistentDataPath, _fileName);
        dataControllableObjects = FindAllDataControllables();
        LoadGame();
    }

    public void SaveGame()
    {
        foreach (var obj in dataControllableObjects)
            obj.SaveData(ref _database);

        fileDataHandler.Save(_database);
    }

    public void LoadGame()
    {
        Database loadedDatabase = fileDataHandler.Load();
        if (loadedDatabase != null)
            _database = loadedDatabase;

        foreach (var obj in dataControllableObjects)
            obj.LoadData(_database);
    }

    List<IDataControllable> FindAllDataControllables()
    {
        return FindObjectsOfType<MonoBehaviour>(true).OfType<IDataControllable>().ToList();
    }

    public void OpenSavesFolder()
    {
        Process.Start(Application.persistentDataPath);
    }


    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
