using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataControllable
{
    public GameManager Instance { get; private set; }

    public Transform tireHolder;
    public TextMeshProUGUI passedDistanceText;
    public Animator UIAnimator;
    public TextMeshProUGUI timerText;
    public float timeForEndInS;
    [Header("Result's info")]
    public TextMeshProUGUI distanceResult;
    public TextMeshProUGUI moneyEarnedResult;

    private bool _isPlaying = true;
    private GameObject _player;
    private GameObject _playerPrefab;
    private float _passedDistance = 0;
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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void UIClickHandler(GameObject obj)
    {
        Debug.Log(obj.tag);
        if (obj.CompareTag("BackButton"))
            ScenesManager.Instance.SwitchScene("Menu");
    }

    public void LoadData(Database database)
    {
        _playerPrefab = ItemsManager.PathToPrefab<GameObject>(ItemsManager.Instance.GetItemByType(database.selectedTire).path);
        SpawnTire();
    }

    public void SaveData(ref Database database)
    {

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

    private void OnLaunch(float arg1)
    {
        passedDistanceText.gameObject.SetActive(true);
        StartCoroutine(RefreshPassedDistance());
        StartCoroutine(CheckForStop());
    }

    private void EndGame()
    {
        UIAnimator.SetBool("IsResultsOpen", true);

        _earnedMoney = (int) (_passedDistance / 10);
        distanceResult.text = Mathf.CeilToInt(_passedDistance).ToString();
        moneyEarnedResult.text = _earnedMoney.ToString();

        MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount + _earnedMoney);
        LaunchesManager.Instance.ReduceLaunchesAmount();
        DataManager.Instance.SaveGame();
    }

    IEnumerator RefreshPassedDistance()
    {
        while (_isPlaying)
        {
            _passedDistance = _player.transform.position.z - tireHolder.transform.position.z;
            if (_passedDistance < 0)
                _passedDistance = 0;
            passedDistanceText.text = Mathf.CeilToInt(_passedDistance).ToString() + " m.";
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator CheckForStop()
    {
        Rigidbody playerRigidbody = _player.GetComponent<Rigidbody>();
        Vector3 velocityToEnd = new Vector3(1f, 1f, 1f);
        bool isStopping = false;
        float timeToCheckInS = 1f;
        float startTime = Time.time;
        float currentTime;

        while (_isPlaying)
        {
            currentTime = Time.time;
            Vector3 playerVelocity = playerRigidbody.velocity;
            //Debug.Log($"Distance: {Vector3.Distance(playerVelocity, velocityToEnd)}");
            //Debug.Log($"Magnitude: {velocityToEnd.magnitude}");

            if (Vector3.Distance(playerVelocity, velocityToEnd) <= velocityToEnd.magnitude)
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
}
