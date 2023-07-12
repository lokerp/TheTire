using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataControllable, IAchievementsControllable
{
    public GameManager Instance { get; private set; }
    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }

    [Header("Spawn Holders")]
    public Transform tireHolder;
    public Transform weaponHolder;
    public Animator launchAnimator;


    [Header("Pages And UI'S")]
    [SerializeField] private Page _pausePage;
    [SerializeField] private AcceptPage _pauseAcceptPage;
    [SerializeField] private Page _scalePage;
    [SerializeField] private Page _timerPage;
    [SerializeField] private Page _passedDistancePage;
    [SerializeField] private Page _resultsPage;
    [SerializeField] private Page _pauseButtonPage;

    [SerializeField] private ButtonHolder _returnButton;
    public TextMeshProUGUI passedDistanceText;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    public float timeForEndInS;
    public AudioSource timerCountdown;

    [Header("Result's info")]
    public TextMeshProUGUI distanceResult;
    public TextMeshProUGUI moneyEarnedResult;
    public GameObject recordHolder;
    public AudioSource newRecordSound;

    [Header("Other")]
    public float timeInSToCloseScale;

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
        _pausePage.OnOpen += Pause;
        _pausePage.OnClose += Unpause;
        _pauseAcceptPage.OnAccept += ExitToMenu;
        _returnButton.OnClick += ExitToMenu;
    }


    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunch;
        _pausePage.OnOpen -= Pause;
        _pausePage.OnClose -= Unpause;
        _pauseAcceptPage.OnAccept -= ExitToMenu;
        _returnButton.OnClick -= ExitToMenu;
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
        _pauseButtonPage.Close();
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
        launchAnimator.runtimeAnimatorController = _weapon.GetComponent<Weapon>().animatorController;
        launchAnimator.SetTrigger("HitPrepare");
    }

    private void OnLaunch(Vector3 force, float forceModifier)
    {
        _pauseButtonPage.Open();
        _passedDistancePage.Open();
        StartCoroutine(RefreshPassedDistance());
        StartCoroutine(CheckForStop());
        StartCoroutine(CloseScale());
    }

    IEnumerator CloseScale()
    {
        yield return new WaitForSeconds(timeInSToCloseScale);
        _scalePage.Close();
        yield break;
    }

    private IEnumerator ShowResults()
    {
        _resultsPage.Open();
        _timerPage.Close();
        _pauseButtonPage.Close();

        _earnedMoney = (int)(_passedDistance / 10);
        distanceResult.text = Mathf.CeilToInt(_passedDistance).ToString();
        moneyEarnedResult.text = _earnedMoney.ToString();

        if (_passedDistance > _recordDistance)
        {
            recordHolder.SetActive(true);
            AnimationEndHandler resultsAnimationEnd = _resultsPage.GetComponent<AnimationEndHandler>();
            while (resultsAnimationEnd.IsAnimationEnded != true)
                yield return null;
            newRecordSound.Play();
        }
        else
            recordHolder.SetActive(false);

        MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount + _earnedMoney);
    }

    private void Pause()
    {
        Time.timeScale = 0;
        _pauseButtonPage.Close();
        AudioManager.Instance.ChangeVolume(AudioManager.VolumeType.GameSounds, 0);
        timerCountdown.Pause();
    }

    private void Unpause()
    {
        Time.timeScale = 1;
        _pauseButtonPage.Open();
        AudioManager.Instance.ChangeVolume(AudioManager.VolumeType.GameSounds, 1);
        timerCountdown.UnPause();
    }

    private void ExitToMenu()
    {
        Unpause();
        ScenesManager.Instance.SwitchScene("Menu");
    }

    IEnumerator RefreshPassedDistance()
    {
        float maxHeight = 0;
        float maxDistance = 0;
        float mapSizeFromPlayer = 3200 - _player.transform.position.z;
        AchievementInfo astronautAchievement = AchievementsManager.Instance.GetAchievementInfoById(2);
        AchievementInfo firstKilometerAchievement = AchievementsManager.Instance.GetAchievementInfoById(3);
        AchievementInfo timeFliesAchievement = AchievementsManager.Instance.GetAchievementInfoById(4);
        AchievementInfo championAchievement = AchievementsManager.Instance.GetAchievementInfoById(6);
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
            if (maxDistance < _passedDistance)
            {
                maxDistance = _passedDistance;
                GetFirstKilometerAchievement(maxDistance, firstKilometerAchievement);
                GetChampionAchievement((int) (maxDistance / 1000), championAchievement);
            }
            if (maxDistance >= mapSizeFromPlayer)
                GetTimeFliesAchievement(1, timeFliesAchievement);

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
                    StartCoroutine(ShowResults());
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
        if (!_timerPage.IsOpened())
        {
            _timerPage.Open();
            timerCountdown.Play();
            _passedDistancePage.Close();
        }

        int timeInS = (int)(timeForEndInS - time) >= 0 ? (int)(timeForEndInS - time) : 0;
        timerText.text = timeInS.ToString();
    }

    void CloseTimer()
    {
        _timerPage.Close();
        timerCountdown.Stop();
        _passedDistancePage.Open();
    }

    void GetAstronautAchievement(float height, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(height, achievement);
        OnAchievementProgressChanged.Invoke(progress, 2);
    }

    void GetFirstKilometerAchievement(float distance, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(distance, achievement);
        OnAchievementProgressChanged.Invoke(progress, 3);
    }

    void GetTimeFliesAchievement(int progress, AchievementInfo achievement)
    {
        OnAchievementProgressChanged.Invoke(new AchievementProgress(progress, true), 4);
    }

    void GetChampionAchievement(int distanceInKm, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(distanceInKm, achievement);
        OnAchievementProgressChanged.Invoke(progress, 6);
    }
}
