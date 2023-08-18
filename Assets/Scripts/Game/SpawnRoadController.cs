using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpawnRoadController : Ston<SpawnRoadController>
{
    [Range(3, 10)] public int maxRoadsSpawned = 4;

    public List<RoadCoords> roadsToSpawn = new();
    public RoadCoords startRoad;
    public float worldCycleSize;
    private List<RoadCoords> _spawnedRoads = new();

    private int _frontSpawnedCount;
    private int _backSpawnedCount;
    private int _maxFrontSpawnedCount;
    private int _maxBackSpawnedCount;

    private int _roadsShouldSpawnCount;

    private int _roadLength;

    private int _startIndex = -1;
    private int _endIndex = -1;
    private SpawnWays _spawnWay;

    private Transform _playerPos;
  
    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        foreach (var road in roadsToSpawn)
            if (road != startRoad)
                road.gameObject.SetActive(false);

        _frontSpawnedCount = 1;
        _backSpawnedCount = 0;

        _maxFrontSpawnedCount = maxRoadsSpawned - 1;
        _maxBackSpawnedCount = 1;

        _spawnedRoads.Add(startRoad);

        _startIndex = roadsToSpawn.IndexOf(_spawnedRoads.First());
        _endIndex = _startIndex;

        while (ShouldSpawn())
            Spawn();

        if (roadsToSpawn.Count < maxRoadsSpawned)
            throw new System.Exception("roadsToSpawn < maxRoadSpawned!!!");
    }

    private void OnEnable()
    {
        WorldLoopController.OnLoop += OnLoopHandler;
    }

    private void OnDisable()
    {
        WorldLoopController.OnLoop -= OnLoopHandler;
    }

    void Update()
    {
        while (ShouldSpawn())
        {
            Spawn();
            if (ShouldDespawn())
                Despawn();
        }
    }

    void OnLoopHandler(Vector3 translateVec)
    {
        foreach (var road in _spawnedRoads)
            road.transform.Translate(translateVec, Space.World);
    }

    void Spawn()
    {
        RoadCoords _spawnedObject = null;

        if (_spawnWay == SpawnWays.Front)
        {
            _endIndex = MathfExtension.Mod(_endIndex + 1, roadsToSpawn.Count);
            _spawnedObject = roadsToSpawn[_endIndex];
            _spawnedObject.gameObject.SetActive(true);

            _spawnedObject.transform.position = CalculatePosition(_spawnedObject, _spawnedRoads.Last());
            _spawnedRoads.Add(_spawnedObject);
            _frontSpawnedCount++;
        }

        else if (_spawnWay == SpawnWays.Back)
        {
            _startIndex = MathfExtension.Mod(_startIndex - 1, roadsToSpawn.Count);
            _spawnedObject = roadsToSpawn[_startIndex];
            _spawnedObject.gameObject.SetActive(true);

            _spawnedObject.transform.position = CalculatePosition(_spawnedObject, _spawnedRoads.First());
            _spawnedRoads.Insert(0, _spawnedObject);
            _backSpawnedCount++;
        }

        //Debug.Log($"Spawned: SpawnWay {_spawnWay} - {_spawnedObject.gameObject}");
    }

    void Despawn()
    {
        if (_spawnWay == SpawnWays.Front)
        {
            _startIndex++;
            _spawnedRoads[0].gameObject.SetActive(false);
            _spawnedRoads.RemoveAt(0);
            _frontSpawnedCount--;
        }

        else if (_spawnWay == SpawnWays.Back)
        {
            _endIndex--;
            _spawnedRoads.Last().gameObject.SetActive(false);
            _spawnedRoads.RemoveAt(_spawnedRoads.Count - 1);
            _backSpawnedCount--;
        }

        //Debug.Log($"Despawned: {(_spawnWay == SpawnWay.Front ? SpawnWay.Back : _spawnWay)}");
    }

    bool ShouldSpawn()
    {
        if (_frontSpawnedCount < _maxFrontSpawnedCount 
         || _playerPos.position.z >= _spawnedRoads[2].Begin.transform.position.z)
        {
            _spawnWay = SpawnWays.Front;
            return true;
        }

        else if (_backSpawnedCount < _maxBackSpawnedCount
              || _playerPos.position.z < _spawnedRoads[1].Begin.transform.position.z)
        {
            _spawnWay = SpawnWays.Back;
            return true;
        }

        return false;
    }

    bool ShouldDespawn()
    {
        if (_spawnedRoads.Count > maxRoadsSpawned)
            return true;
        return false;
    }

    Vector3 CalculatePosition(RoadCoords newRoad, RoadCoords lastRoad)
    {
        Vector3 newPosition = new();

        switch (_spawnWay)
        {
            case SpawnWays.Front:
                newPosition = new Vector3(newRoad.transform.position.x,
                                          newRoad.transform.position.y,
                                          lastRoad.End.position.z
                                          + newRoad.transform.position.z
                                          - newRoad.Begin.position.z);
                break;
            case SpawnWays.Back:
                newPosition = new Vector3(newRoad.transform.position.x,
                                          newRoad.transform.position.y,
                                          lastRoad.Begin.position.z
                                          + newRoad.transform.position.z
                                          - newRoad.End.position.z);
                break;

        }

        return newPosition;
    }
}
