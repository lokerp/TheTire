using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnRoad : MonoBehaviour
{
    public int maxRoadSpawned = 4;
    private int currentRoadSpawned;
    private int spawnIndex;
    private int despawnIndex;
    private GameObject spawnedObject;
    private GameObject despawnedObject;
    public List<GameObject> roadsToSpawn = new List<GameObject>();
    private List<spawnedRoad> spawnedRoads;
    public Transform playerPos;
    PoolManager poolManager;

    struct spawnedRoad
    {
        public GameObject road;
        public RoadCoords roadCoords;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnedRoads = FindSpawnedRoads();
        currentRoadSpawned = spawnedRoads.Count;
        spawnIndex = currentRoadSpawned - 1;

        Debug.Log(currentRoadSpawned);

        if (roadsToSpawn.Count < maxRoadSpawned)
            throw new System.Exception("roadsToSpawn < maxRoadSpawned!!!");

        if (currentRoadSpawned > maxRoadSpawned)
            throw new System.Exception("currentRoadSpawned > maxRoadSpawned!!!");

        poolManager = new(roadsToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldSpawn())
        {
            if (currentRoadSpawned == roadsToSpawn.Count)
            {
                spawnedRoads[0].road.transform.position = calculatePosition(spawnedRoads[0], spawnedRoads.Last());

                spawnedRoad temp = spawnedRoads[0];
                spawnedRoads.Add(temp);
                spawnedRoads.RemoveAt(0);
            }

            else
            {
                spawnedObject = poolManager.SpawnUnused(spawnIndex, out spawnIndex, true);

                Debug.Log(spawnedObject);
                spawnedRoad newRoad = new()
                {
                    road = spawnedObject,
                    roadCoords = spawnedObject.GetComponent<RoadCoords>()
                };

                newRoad.road.transform.position = calculatePosition(newRoad, spawnedRoads.Last());
                spawnedRoads.Add(newRoad);
            }
        }
        if (ShouldDespawn())
        {
            poolManager.DespawnOldest(out despawnIndex);
            spawnedRoads.RemoveAt(0);
        }
    }

    List<spawnedRoad> FindSpawnedRoads()
    {
        List<spawnedRoad> roads = new();
        List<GameObject> uniqueRoads = roadsToSpawn.Distinct().ToList();

        foreach (GameObject r in uniqueRoads) {
            if (r.activeInHierarchy)
            {
                spawnedRoad road;
                road.road = r;
                road.roadCoords = r.GetComponent<RoadCoords>();
                roads.Add(road);
            }
        }

        return roads;
    }

    bool ShouldSpawn()
    {
        if (playerPos.position.z >= spawnedRoads[1].roadCoords.Begin.position.z 
            || spawnedRoads.Count < maxRoadSpawned)
            return true;
        return false;
    }

    bool ShouldDespawn()
    {
        if (spawnedRoads.Count > maxRoadSpawned)
            return true;
        return false;
    }

    Vector3 calculatePosition(spawnedRoad newRoad, spawnedRoad lastRoad)
    {
        return new Vector3(newRoad.road.transform.position.x,
                           newRoad.road.transform.position.y,
                           lastRoad.roadCoords.End.position.z
                           + newRoad.road.transform.position.z
                           - newRoad.roadCoords.Begin.position.z);
    }
}
