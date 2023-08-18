using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    private PoolManager poolManager;
    private List<GameObject> skiesToSpawn = new();
    private List<GameObject> spawnedSkies = new();
    private float timeToCheckInSeconds = .4f;
    public int minHeight = 80;
    public int maxHeight = 130;
    public int spreadRadius = 120;
    public int sphereDiameter = 900;
    public Material material;
    [Range(0, 10)]public float scaleMinAmount = 1f;
    [Range(0, 10)]public float scaleMaxAmount = 5f;

    private int sphereRadius;
    private Transform _playerTransform;
    private Rigidbody _playerRb;
    private SpawnWays _spawnWay;

    private void Awake()
    {
        sphereRadius = sphereDiameter / 2;
    }

    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _playerRb = _playerTransform.GetComponent<Rigidbody>();
        _spawnWay = SpawnWays.Front;
        foreach (Transform sky in transform)
            skiesToSpawn.Add(sky.gameObject);

        poolManager = new PoolManager(skiesToSpawn);
        SpawnSkiesAtStart(_playerTransform.position);
        spawnedSkies = FindSpawnedSkies();

        StartCoroutine(SpawnSkies());
    }

    private void OnEnable()
    {
        WorldLoopController.OnLoop += OnLoopHandler;
    }

    private void OnDisable()
    {
        WorldLoopController.OnLoop -= OnLoopHandler;
    }

    void OnLoopHandler(Vector3 translateVec)
    {
        foreach (var cloud in spawnedSkies)
            cloud.transform.Translate(translateVec, Space.World);
    }

    GameObject ShouldSpawn() 
    {
        if (_playerRb.velocity.z < 0)
            _spawnWay = SpawnWays.Back;
        else
            _spawnWay = SpawnWays.Front;

        if (_spawnWay == SpawnWays.Back)
        {
            foreach (var sky in spawnedSkies)
                if (sky.transform.position.z > _playerTransform.position.z + sphereRadius + 50)
                    return sky;
        }

        else if (_spawnWay == SpawnWays.Front)
        {
            foreach (var sky in spawnedSkies)
                if (sky.transform.position.z < _playerTransform.position.z - 30)
                    return sky;
        }

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
                                                     Random.Range(playerPos.z + 100, playerPos.z + sphereRadius));

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
            Vector3 playerPos = _playerTransform.position;
            GameObject sky;
            if (sky = ShouldSpawn())
            {
                float newSkyZCoord = Random.Range(playerPos.z + sphereRadius + 50,
                                                  playerPos.z + sphereDiameter);
                if (_spawnWay == SpawnWays.Back)
                    newSkyZCoord = 2 * playerPos.z - newSkyZCoord;

                sky.transform.position = new Vector3(Random.Range(-spreadRadius, spreadRadius),
                                                     Random.Range(Mathf.Clamp(playerPos.y - heightsAmplitude, minHeight, int.MaxValue),
                                                                  Mathf.Clamp(playerPos.y + heightsAmplitude, minHeight, int.MaxValue)),
                                                     newSkyZCoord);
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
