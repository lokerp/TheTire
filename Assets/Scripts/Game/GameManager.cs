using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataControllable, IAchievementsControllable
{
    public GameManager Instance { get; private set; }
    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }

    public Transform tireHolder;
    public Transform weaponHolder;
    public TextMeshProUGUI passedDistanceText;
    public Animator UIAnimator;
    public TextMeshProUGUI timerText;
    public float timeForEndInS;
    [Header("Result's info")]
    public TextMeshProUGUI distanceResult;
    public TextMeshProUGUI moneyEarnedResult;
    public GameObject recordHolder;

    private bool _isPlaying = true;
    private GameObject _player;
    private GameObject _playerPrefab;
    private GameObject _weapon;
    private GameObject _weaponPrefab;
    private float _passedDistance = 0;
    private float _recordDistance = 0;
    private int _earnedMoney = 0;

    private void OnEnable()
    {
        LaunchController.OnLaunch += OnLaunch;
        UIEvents.OnUIClick += UIClickHandler;
    }

    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunch;
        UIEvents.OnUIClick -= UIClickHandler;
    }

    private void OnDestroy()
    {
        OnAchievementProgressChanged = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void UIClickHandler(GameObject obj)
    {
        if (obj.CompareTag("BackButton"))
            ScenesManager.Instance.SwitchScene("Menu");
    }

    public void LoadData(Database database)
    {
        _playerPrefab = ItemsManager.PathToPrefab<GameObject>(ItemsManager.Instance.GetItemByType(database.selectedTire).path);
        _weaponPrefab = ItemsManager.PathToPrefab<GameObject>(ItemsManager.Instance.GetItemByType(database.selectedWeapon).path);
        _recordDistance = database.records.RecordDistance;
        SpawnTire();
        SpawnWeapon();
    }

    public void SaveData(ref Database database)
    {
        if (_passedDistance > _recordDistance)
            database.records.RecordDistance = _passedDistance;
    }

    void SpawnTire()
    {
        GameObject spawnedObj = Instantiate(_playerPrefab, tireHolder, false);
        spawnedObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
                                                           | RigidbodyConstraints.FreezePositionZ
                                                           | RigidbodyConstraints.FreezeRotation;
        spawnedObj.AddComponent<PlayerController>();

        _player = spawnedObj;
    }

    private void SpawnWeapon()
    {
        GameObject spawnedObj = Instantiate(_weaponPrefab, weaponHolder, false);
        Rigidbody spawnedObjRb = spawnedObj.GetComponent<Rigidbody>();

        spawnedObjRb.constraints = RigidbodyConstraints.None;
        spawnedObjRb.isKinematic = true;

        _weapon = spawnedObj;
        _weapon.GetComponent<Animator>().SetTrigger("HitPrepare");
    }

    private void OnLaunch(Vector3 force, float forceModifier)
    {
        passedDistanceText.gameObject.SetActive(true);
        StartCoroutine(RefreshPassedDistance());
        StartCoroutine(CheckForStop());
    }

    private void EndGame()
    {
        UIAnimator.SetBool("IsResultsOpen", true);

        if (_passedDistance > _recordDistance)
            recordHolder.SetActive(true);
        else
            recordHolder.SetActive(false);

        _earnedMoney = (int) (_passedDistance / 10);
        distanceResult.text = Mathf.CeilToInt(_passedDistance).ToString();
        moneyEarnedResult.text = _earnedMoney.ToString();

        MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount + _earnedMoney);
        DataManager.Instance.SaveGame();
    }

    IEnumerator RefreshPassedDistance()
    {
        float maxHeight = 0;
        AchievementInfo astronautAchievement = AchievementsManager.Instance.GetAchievementInfoById(2);
        while (_isPlaying)
        {
            Vector3 distanceVector = _player.transform.position - tireHolder.transform.position;
            _passedDistance = distanceVector.z;
            if (_passedDistance < 0)
                _passedDistance = 0;
            passedDistanceText.text = Mathf.CeilToInt(_passedDistance).ToString() + " m.";

            if (maxHeight < distanceVector.y)
            {
                maxHeight = distanceVector.y;
                GetAstronautAchievement(maxHeight, astronautAchievement);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator CheckForStop()
    {
        Transform playerTransform = _player.GetComponent<Transform>();
        Vector3 startPosition = playerTransform.position;
        float lengthToEnd = new Vector3(1f, 1f, 1f).magnitude;
        bool isStopping = false;
        float timeToCheckInS = 1f;
        float startTime = Time.time;
        float currentTime;

        while (_isPlaying)
        {
            currentTime = Time.time;
            Vector3 playerPosition = playerTransform.position;
            //Debug.Log($"Distance: {Vector3.Distance(playerVelocity, velocityToEnd)}");
            //Debug.Log($"Magnitude: {velocityToEnd.magnitude}");

            if (Vector3.Distance(startPosition, playerPosition) <= lengthToEnd)
            {
                if (currentTime - startTime >= timeToCheckInS)
                {
                    isStopping = true;
                    StartOrRefreshTimer(currentTime - startTime - timeToCheckInS);
                }

                if (currentTime - startTime >= timeToCheckInS + timeForEndInS)
                {
                    _isPlaying = false;
                    EndGame();
                }
            }
            else
            {
                startTime = currentTime;
                startPosition = playerPosition;

                if (isStopping)
                {
                    isStopping = false;
                    CloseTimer();
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void StartOrRefreshTimer(float time)
    {
        if (UIAnimator.GetBool("IsTimerOpen") == false)
            UIAnimator.SetBool("IsTimerOpen", true);

        int timeInS = (int)(timeForEndInS - time) >= 0 ? (int)(timeForEndInS - time) : 0;
        timerText.text = timeInS.ToString();
    }

    void CloseTimer()
    {
        if (UIAnimator.GetBool("IsTimerOpen") == true)
            UIAnimator.SetBool("IsTimerOpen", false);
    }

    void GetAstronautAchievement(float height, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(height, achievement);
        OnAchievementProgressChanged.Invoke(progress, 2);
    }
}
