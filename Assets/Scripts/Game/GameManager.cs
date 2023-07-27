using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Ston<GameManager>, IDataControllable, IAchievementsControllable
{
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
    [SerializeField] private Page _rewindPage;

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

    [Header("Rewind")]
    public Color rewindOnColor;
    public Color rewindOffColor;
    public ButtonHolder rewindButton;
    public float timeToShowRewindBtn = 10f;
    public float minRewindTimeScale = 1.5f;
    public float maxRewindTimeScale = 6;
    public float maxRewindVelocity;

    [Header("Other")]
    public float timeInSToCloseScale;
    public PhysicMaterial tirePhysicMaterial;
    [Range(0, 1)] public float bouncinessMaxLvl;

    private float _currentTimeScale = 1;
    private bool _isRewindEnabled = false;
    private bool _isPlaying = true;
    private GameObject _player;
    private GameObject _playerPrefab;
    private Rigidbody _playerRb;
    private PlayerController _playerContr;
    private GameObject _weapon;
    private GameObject _weaponPrefab;
    private float _passedDistance = 0;
    private float _recordDistance = 0;
    private int _earnedMoney = 0;
    private int _launchesCount = 0;

    private void OnEnable()
    {
        LaunchController.OnLaunch += OnLaunch;
        _pausePage.OnOpen += Pause;
        _pausePage.OnClose += Unpause;
        _pauseAcceptPage.OnAccept += ExitToMenu;
        _returnButton.OnClick += ExitToMenu;
        rewindButton.OnClick += TurnRewind;
    }


    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunch;
        _pausePage.OnOpen -= Pause;
        _pausePage.OnClose -= Unpause;
        _pauseAcceptPage.OnAccept -= ExitToMenu;
        _returnButton.OnClick -= ExitToMenu;
        rewindButton.OnClick -= TurnRewind;
    }

    private void OnDestroy()
    {
        OnAchievementProgressChanged = null;
    }

    protected override void Awake()
    {
        base.Awake();
        _pauseButtonPage.Close();
    }

    public void LoadData(Database database)
    {
        _playerPrefab = ItemsManager.PathToPrefab<GameObject>(ItemsManager.Instance.GetItemByType(database.selectedTire).path);
        _weaponPrefab = ItemsManager.PathToPrefab<GameObject>(ItemsManager.Instance.GetItemByType(database.selectedWeapon).path);
        _recordDistance = database.records.RecordDistance;
        _launchesCount = database.totalLaunchesCount;
        SetTireBounciness(database.bouncinessLevel, UpgradesPage.maxBouncinessLevel);
        SpawnTire();
        SpawnWeapon();
    }

    public void SaveData(ref Database database)
    {
        if (!_isPlaying && _passedDistance > _recordDistance)
        {
            database.records.RecordDistance = _passedDistance;
            DataManager.Instance.SetLeaderboardScore((int) _passedDistance);
        }
    }

    void SetTireBounciness(int curLvl, int maxLvl)
    {
        tirePhysicMaterial.bounciness = (float) curLvl / maxLvl * bouncinessMaxLvl;
        tirePhysicMaterial.dynamicFriction = (1 - (float)curLvl / maxLvl) * 1000;
    }

    void SpawnTire()
    {
        GameObject spawnedObj = Instantiate(_playerPrefab, tireHolder, false);
        spawnedObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
                                                           | RigidbodyConstraints.FreezePositionZ
                                                           | RigidbodyConstraints.FreezeRotation;
        _playerContr = spawnedObj.AddComponent<PlayerController>();
        _playerContr.OnCollision += OnCollisionAchievementsHandler;

        _player = spawnedObj;
        _playerRb = _player.GetComponent<Rigidbody>();
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
        GetGamerAchievement(_launchesCount, AchievementsManager.Instance.GetAchievementInfoById(9));
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
        AudioManager.Instance.gameMusic.Stop();
        TurnRewind(false);
        _resultsPage.Open();
        _timerPage.Close();
        _pauseButtonPage.Close();
        _rewindPage.Close();

        _earnedMoney = (int)(_passedDistance / 10) * 2;
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
        AudioManager.Instance.ChangeVolume(VolumeTypes.GameSounds, 0);
        AudioManager.Instance.gameMusic.Pause();
        timerCountdown.Pause();
    }

    private void Unpause()
    {
        Time.timeScale = _currentTimeScale;
        _pauseButtonPage.Open();
        AudioManager.Instance.ChangeVolume(VolumeTypes.GameSounds, 1);
        AudioManager.Instance.gameMusic.UnPause();
        timerCountdown.UnPause();
    }

    private void ExitToMenu()
    {
        Unpause();
        TurnRewind(false);
        ScenesManager.Instance.SwitchScene("Menu");
    }

    IEnumerator RefreshPassedDistance()
    {
        float timePassed = 0;
        float maxHeight = 0;
        float maxDistanceInM = 0;
        float mapSizeFromPlayer = 3200 - _player.transform.position.z;
        AchievementInfo pilotAchievement = AchievementsManager.Instance.GetAchievementInfoById(7);
        AchievementInfo astronautAchievement = AchievementsManager.Instance.GetAchievementInfoById(2);
        AchievementInfo firstKilometerAchievement = AchievementsManager.Instance.GetAchievementInfoById(3);
        AchievementInfo timeFliesAchievement = AchievementsManager.Instance.GetAchievementInfoById(4);
        AchievementInfo championAchievement = AchievementsManager.Instance.GetAchievementInfoById(6);
        AchievementInfo halfMarathonAchievement = AchievementsManager.Instance.GetAchievementInfoById(8);

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
                GetPilotAchievement(maxHeight, pilotAchievement);
            }
            if (maxDistanceInM < _passedDistance)
            {
                maxDistanceInM = _passedDistance;
                var maxDistanceInKm = (int)(maxDistanceInM / 1000);
                GetFirstKilometerAchievement(maxDistanceInM, firstKilometerAchievement);
                GetChampionAchievement(maxDistanceInKm, championAchievement);
                GetHalfMarathonAchievement(maxDistanceInKm, halfMarathonAchievement);

            }
            if (maxDistanceInM >= mapSizeFromPlayer)
                GetTimeFliesAchievement(1, timeFliesAchievement);

            if (!_rewindPage.IsOpened() && timePassed >= timeToShowRewindBtn)
                _rewindPage.Open();

            if (_isRewindEnabled)
            {
                _currentTimeScale = minRewindTimeScale 
                                  + MathfExtension.FuncSpeedApply(_playerRb.velocity.magnitude / maxRewindVelocity,
                                                                  MathfExtension.FuncSpeed.SquareRoot)
                                  * (maxRewindTimeScale - minRewindTimeScale);
                Time.timeScale = _currentTimeScale;
            }

            timePassed += 0.2f;
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

    void TurnRewind()
    {
        var buttonImg = rewindButton.gameObject.GetComponent<Image>();
        if (_isRewindEnabled)
        {
            buttonImg.color = rewindOffColor;
            _isRewindEnabled = false;
            _currentTimeScale = 1;
            Time.timeScale = _currentTimeScale;
        }
        else
        {
            buttonImg.color = rewindOnColor;
            _isRewindEnabled = true;
        }
    }

    void TurnRewind(bool turnOn)
    {
        var buttonImg = rewindButton.gameObject.GetComponent<Image>();
        if (!turnOn)
        {
            buttonImg.color = rewindOffColor;
            _isRewindEnabled = false;
            _currentTimeScale = 1;
            Time.timeScale = _currentTimeScale;
        }
        else
        {
            buttonImg.color = rewindOnColor;
            _isRewindEnabled = true;
        }
    }

    void OnCollisionAchievementsHandler(GameObject collider, Vector3 _, Vector3 __)
    {
        if (collider.CompareTag("Water"))
            GetDiverAchievement();
        else if (collider.CompareTag("Cloud"))
            GetTodayIsCloudyAchievement();
    }

    private void GetDiverAchievement()
    {
        var progress = new AchievementProgress(1, true);
        OnAchievementProgressChanged.Invoke(progress, 0);
    }

    private void GetTodayIsCloudyAchievement()
    {
        var progress = new AchievementProgress(1, true);
        OnAchievementProgressChanged.Invoke(progress, 1);
    }

    void GetGamerAchievement(int launches, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(launches, achievement);
        OnAchievementProgressChanged.Invoke(progress, 9);
    }

    void GetPilotAchievement(float height, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(height, achievement);
        OnAchievementProgressChanged.Invoke(progress, 7);
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

    void GetHalfMarathonAchievement(int distanceInKm, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(distanceInKm, achievement);
        OnAchievementProgressChanged.Invoke(progress, 8);
    }

    public void AfterDataLoaded(Database database) { }
}
