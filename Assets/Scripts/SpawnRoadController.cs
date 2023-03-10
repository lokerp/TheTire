using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpawnRoadController : MonoBehaviour
{
    struct SpawnedRoad
    {
        public GameObject roadGO;
        public RoadCoords roadCoords;
    }

    enum SpawnWay
    {
        Front,
        Back
    }

    [Range(3, 10)] public int maxRoadSpawned = 4;
    public List<GameObject> roadsToSpawn = new();

    private List<SpawnedRoad> _spawnedRoads = new();
    private int _RoadSpawnedCount;
    private int _startIndex = -1;
    private int _endIndex = -1;
    private SpawnWay _spawnWay;

    PoolManager poolManager;

    private Transform _playerPos;

    // Start is called before the first frame update
    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        GetSpawnedRoadList();
        _RoadSpawnedCount = _spawnedRoads.Count;
        
        _startIndex = roadsToSpawn.IndexOf(_spawnedRoads.First().roadGO);
        _endIndex = roadsToSpawn.IndexOf(_spawnedRoads.Last().roadGO);

        Debug.Log(_RoadSpawnedCount);

        if (roadsToSpawn.Count < maxRoadSpawned)
            throw new System.Exception("roadsToSpawn < maxRoadSpawned!!!");

        if (_RoadSpawnedCount > maxRoadSpawned)
            throw new System.Exception("currentRoadSpawned > maxRoadSpawned!!!");

        poolManager = new(roadsToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldSpawn())
        {
            Spawn();
        }
        if (ShouldDespawn())
        {
            Despawn();
        }
    }

    void GetSpawnedRoadList()
    {
        foreach (var rGO in roadsToSpawn.Distinct())
        {
            if (!rGO.activeInHierarchy)
                continue;

            SpawnedRoad r;
            r.roadGO = rGO;
            r.roadCoords = rGO.GetComponent<RoadCoords>();
            _spawnedRoads.Add(r);
        }
    }

    void Spawn()
    {
        GameObject _spawnedObject = null;
        Debug.Log(_spawnWay);

        if (_spawnWay == SpawnWay.Front)
        {
            _endIndex = Mod(_endIndex + 1, roadsToSpawn.Count);
            _spawnedObject = roadsToSpawn[_endIndex];
            _spawnedObject.SetActive(true);

            SpawnedRoad newRoad = new()
            {
                roadGO = _spawnedObject,
                roadCoords = _spawnedObject.GetComponent<RoadCoords>()
            };

            newRoad.roadGO.transform.position = CalculatePosition(newRoad, _spawnedRoads.Last());
            _spawnedRoads.Add(newRoad);
        }

        else if (_spawnWay == SpawnWay.Back)
        {
            _startIndex = Mod(_startIndex - 1, roadsToSpawn.Count);
            _spawnedObject = roadsToSpawn[_startIndex];
            _spawnedObject.SetActive(true);

            SpawnedRoad newRoad = new()
            {
                roadGO = _spawnedObject,
                roadCoords = _spawnedObject.GetComponent<RoadCoords>()
            };

            newRoad.roadGO.transform.position = CalculatePosition(newRoad, _spawnedRoads.First());
            _spawnedRoads.Insert(0, newRoad); 
        }

        Debug.Log(_spawnedObject);
    }

    void Despawn()
    {
        if (_spawnWay == SpawnWay.Front)
        {
            _startIndex++;
            _spawnedRoads[0].roadGO.SetActive(false);
            _spawnedRoads.RemoveAt(0);
        }

        else if (_spawnWay == SpawnWay.Back)
        {
            _endIndex--;
            _spawnedRoads.Last().roadGO.SetActive(false);
            _spawnedRoads.RemoveAt(_spawnedRoads.Count - 1);
        }

        Debug.Log($"Despawned: {_spawnWay}");
    }

    bool ShouldSpawn()
    {
        if (_playerPos.position.z >= _spawnedRoads[2].roadCoords.Begin.transform.position.z)
        {
            _spawnWay = SpawnWay.Front;
            return true;
        }

        else if (_playerPos.position.z < _spawnedRoads[1].roadCoords.Begin.transform.position.z)
        {
            _spawnWay = SpawnWay.Back;
            return true;
        }

        return false;
    }

    bool ShouldDespawn()
    {
        if (_spawnedRoads.Count > maxRoadSpawned)
            return true;
        return false;
    }

    Vector3 CalculatePosition(SpawnedRoad newRoad, SpawnedRoad lastRoad)
    {
        Vector3 newPosition = new();

        switch (_spawnWay)
        {
            case SpawnWay.Front:
                newPosition = new Vector3(newRoad.roadGO.transform.position.x,
                                          newRoad.roadGO.transform.position.y,
                                          lastRoad.roadCoords.End.position.z
                                          + newRoad.roadGO.transform.position.z
                                          - newRoad.roadCoords.Begin.position.z);
                break;
            case SpawnWay.Back:
                newPosition = new Vector3(newRoad.roadGO.transform.position.x,
                                          newRoad.roadGO.transform.position.y,
                                          lastRoad.roadCoords.Begin.position.z
                                          + newRoad.roadGO.transform.position.z
                                          - newRoad.roadCoords.End.position.z);
                break;

        }

        return newPosition;
    }

    private int Mod(int a, int b)
    {
        int c = a % b;
        return c < 0 ? c += b : c;
    }
}
