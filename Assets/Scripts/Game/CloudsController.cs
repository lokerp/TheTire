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
    public int sphereDiameter = 900;
    public Material material;
    [Range(0, 10)]public float scaleMinAmount = 1f;
    [Range(0, 10)]public float scaleMaxAmount = 5f;

    private Transform playerTransform;


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (Transform sky in transform)
            skiesToSpawn.Add(sky.gameObject);

        poolManager = new PoolManager(skiesToSpawn);
        SpawnSkiesAtStart(playerTransform.position);
        spawnedSkies = FindSpawnedSkies();

        StartCoroutine(SpawnSkies());
    }

    GameObject ShouldSpawn() 
    {
        foreach (var sky in spawnedSkies)
            if (sky.transform.position.z < playerTransform.position.z - 30)
                return sky;

        return null;
    }

    void SpawnSkiesAtStart(Vector3 playerPos)
    {
        GameObject sky;

        while (true)
        {
            sky = poolManager.SpawnYoungest();
            if (sky)
            {
                sky.transform.position = new Vector3(Random.Range(-spreadRadius, spreadRadius),
                                                     Random.Range(minHeight, maxHeight),
                                                     Random.Range(playerPos.z + 100, playerPos.z + sphereDiameter / 2));

                SetRandomTransform(sky.transform);
                spawnedSkies.Add(sky);
            }
            else
                break;
        }
    }

    IEnumerator SpawnSkies()
    {
        int heightsAmplitude = maxHeight - minHeight;
        while (true)
        {
            Vector3 playerPos = playerTransform.position;
            GameObject sky;
            if (sky = ShouldSpawn())
            {
                sky.transform.position = new Vector3(Random.Range(-spreadRadius, spreadRadius),
                                                     Random.Range(Mathf.Clamp(playerPos.y - heightsAmplitude, minHeight, int.MaxValue),
                                                                  Mathf.Clamp(playerPos.y + heightsAmplitude, minHeight, int.MaxValue)),
                                                     Random.Range(playerPos.z + sphereDiameter / 2 + 50, playerPos.z + sphereDiameter));

                SetRandomTransform(sky.transform);
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

    void SetRandomTransform(Transform sky)
    {
        sky.localScale = new Vector3(Random.Range(scaleMinAmount, scaleMaxAmount),
                                     sky.localScale.x,
                                     Random.Range(scaleMinAmount, scaleMaxAmount));

        sky.transform.rotation = Quaternion.Euler(sky.transform.rotation.z,
                                                  Random.Range(0, 180),
                                                  sky.transform.rotation.z);
    }
}
