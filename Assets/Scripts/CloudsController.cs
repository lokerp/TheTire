using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    private PoolManager poolManager;
    private List<GameObject> skiesToSpawn = new();
    private List<GameObject> spawnedSkies = new();
    private int spawnIndex;
    private float timeToCheckInSeconds = .4f;
    public int minHeight = 80;
    public int maxHeight = 130;
    public int spreadRadius = 120;
    public int sphereRadius = 900;
    private Transform playerTransform;
    private Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerPos = playerTransform.position;

        foreach (Transform sky in transform)
            skiesToSpawn.Add(sky.gameObject);

        poolManager = new PoolManager(skiesToSpawn);
        SpawnSkiesAtStart(playerPos);
        spawnedSkies = FindSpawnedSkies();

        StartCoroutine("SpawnSkies");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject ShouldSpawn() 
    {
        playerPos = playerTransform.position;

        foreach (var sky in spawnedSkies)
            if (sky.transform.position.z < playerPos.z)
                return sky;

        return null;
    }

    void SpawnSkiesAtStart(Vector3 playerPos)
    {
        GameObject sky;

        while (true)
        {
            sky = poolManager.SpawnYoungest(out spawnIndex);
            if (sky)
            {
                sky.transform.position = new Vector3(Random.Range(-spreadRadius, spreadRadius),
                                                     Random.Range(minHeight, maxHeight),
                                                     Random.Range(playerPos.z + 100, playerPos.z + sphereRadius / 2));
                spawnedSkies.Add(sky);
            }
            else
                break;
        }
    }

    IEnumerator SpawnSkies()
    {
        while (true)
        {
            GameObject sky;
            if (sky = ShouldSpawn())
            {
                Debug.Log(sky);
                sky.transform.position = new Vector3(Random.Range(-spreadRadius, spreadRadius),
                                                     Random.Range(minHeight, maxHeight),
                                                     Random.Range(playerPos.z + sphereRadius / 2 + 50, playerPos.z + sphereRadius / 2 + 400));
            }

            yield return new WaitForSeconds(timeToCheckInSeconds);
        }
    }

    List<GameObject> FindSpawnedSkies()
    {
        List<GameObject> skies = new();
        List<GameObject> uniqueSkies = skiesToSpawn.Distinct().ToList();

        foreach(GameObject sky in uniqueSkies)
            if (sky.activeInHierarchy)
                skies.Add(sky);

        return skies;
    }
}
