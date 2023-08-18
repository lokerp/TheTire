using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
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
    [SerializeField] private AcceptPage _pauseRestartAcceptPage;
    [SerializeField] private AcceptPage _pauseReturnAcceptPage;
    [SerializeField] private Notification _noLaunchesNotification;
    [SerializeField] private Page _scalePage;
    [SerializeField] private Page _timerPage;
    [SerializeField] private Page _passedDistancePage;
    [SerializeField] private Page _resultsPage;
    [SerializeField] private Page _pauseButtonPage;
    [SerializeField] private Page _rewindPage;

    [SerializeField] private ButtonHolder _pauseRestartButton;
    [SerializeField] private ButtonHolder _resultsReturnButton;
    [SerializeField] private ButtonHolder _resultsRestartButton;
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
    public float timeInSToOpenNotification;
    public AudioSource noLaunchesErrorSound;
    public float timeInSToCloseScale;
    public PhysicMaterial tirePhysicMaterial;
    [Range(0, 1)] public float physBouncinessMaxLvl;

    private float _currentTimeScale = 1;
    private bool _isRewindEnabled = false;
    private bool _isPlaying = true;
    private bool _isPaused = false;
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
    private Action<ButtonHolderClickEventArgs> _rewindBtnClickHandler;
    private Action _returnAcceptClickHandler;

    private void OnEnable()
    {
        LaunchController.OnLaunch += OnLaunch;
        _pausePage.OnOpen += Pause;
        _pausePage.OnClose += Unpause;
        _pauseReturnAcceptPage.OnAccept += _returnAcceptClickHandler;
        _pauseRestartAcceptPage.OnAccept += Restart;
        _resultsRestartButton.OnClick += OnResultsRestartButtonClick;
        _pauseRestartButton.OnClick += OnPauseRestartButtonClick;
        _resultsReturnButton.OnClick += ExitToMenu;
        rewindButton.OnClick += _rewindBtnClickHandler;
    }

    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunch;
        _pausePage.OnOpen -= Pause;
        _pausePage.OnClose -= Unpause;
        _pauseReturnAcceptPage.OnAccept -= _returnAcceptClickHandler;
        _pauseRestartAcceptPage.OnAccept -= Restart;
        _pauseRestartButton.OnClick -= OnResultsRestartButtonClick;
        _pauseRestartButton.OnClick -= OnPauseRestartButtonClick;
        _resultsReturnButton.OnClick -= ExitToMenu;
        rewindButton.OnClick -= _rewindBtnClickHandler;
    }

    private void OnDestroy()
    {
        OnAchievementProgressChanged = null;
    }

    protected override void Awake()
    {
        base.Awake();
        _pauseButtonPage.Close();

        _rewindBtnClickHandler = (args) => TurnRewind(!_isRewindEnabled);
        _returnAcceptClickHandler = () => ExitToMenu();
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
            database.records.RecordDistance = _passedDistance;
    }

    void SetTireBounciness(int curLvl, int maxLvl)
    {
        tirePhysicMaterial.bounciness = (float) curLvl / maxLvl * physBouncinessMaxLvl;
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
        StartCoroutine(PlayingRoutine());
        StartCoroutine(CheckForStop());
        StartCoroutine(CloseScale());
        Invoke(nameof(ShowRewindBtn), timeToShowRewindBtn);
    }

    private void OnResultsRestartButtonClick(ButtonHolderClickEventArgs clickInfo)
    {
        if (LaunchesManager.Instance.CanPlay())
            Restart();
        else
            _noLaunchesNotification.Open(null, timeInSToOpenNotification, noLaunchesErrorSound);
    }

    private void OnPauseRestartButtonClick(ButtonHolderClickEventArgs clickInfo)
    {
        if (LaunchesManager.Instance.CanPlay())
            _pauseRestartAcceptPage.Open();
        else
            _noLaunchesNotification.Open(null, timeInSToOpenNotification, noLaunchesErrorSound);
    }

    private void ShowRewindBtn()
    {
        _rewindPage.Open();
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
        _timerPage.Close();
        _pauseButtonPage.Close();
        _rewindPage.Close();

        _earnedMoney = Mathf.Clamp((int) (_passedDistance / 10) * 2, 0, int.MaxValue);
        distanceResult.text = ((long) Mathf.Ceil(_passedDistance)).ToString();
        moneyEarnedResult.text = _earnedMoney.ToString();
        MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount + _earnedMoney);

        _resultsPage.Open();

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
    }

    private void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0;
        _pauseButtonPage.Close();
        AudioManager.Instance.ChangeVolume(VolumeTypes.GameSounds, 0);
        AudioManager.Instance.gameMusic.Pause();
        timerCountdown.Pause();
    }

    private void Unpause()
    {
        _isPaused = false;
        Time.timeScale = _currentTimeScale;
        _pauseButtonPage.Open();
        AudioManager.Instance.ChangeVolume(VolumeTypes.GameSounds, 1);
        AudioManager.Instance.gameMusic.UnPause();
        timerCountdown.UnPause();
    }

    private void ExitToMenu(ButtonHolderClickEventArgs args = null)
    {
        Unpause();
        TurnRewind(false);
        ScenesManager.Instance.SwitchScene("Menu");
    }

    private void Restart()
    {
        Unpause();
        TurnRewind(false);
        LaunchesManager.Instance.OnGameStart();
        ScenesManager.Instance.SwitchScene("Game");
    }

    IEnumerator PlayingRoutine()
    {
        float maxHeight = 0;
        float maxDistanceInM = 0;
        float mapSizeFromPlayer = 3200 - _player.transform.position.z;
        AchievementInfo pilotAchievement = AchievementsManager.Instance.GetAchievementInfoById(7);
        AchievementInfo astronautAchievement = AchievementsManager.Instance.GetAchievementInfoById(2);
        AchievementInfo firstKilometerAchievement = AchievementsManager.Instance.GetAchievementInfoById(3);
        AchievementInfo timeFliesAchievement = AchievementsManager.Instance.GetAchievementInfoById(4);
        AchievementInfo championAchievement = AchievementsManager.Instance.GetAchievementInfoById(6);
        AchievementInfo halfMarathonAchievement = AchievementsManager.Instance.GetAchievementInfoById(8);
        AchievementInfo jubileeAchievement = AchievementsManager.Instance.GetAchievementInfoById(13);
        AchievementInfo fastAchievement = AchievementsManager.Instance.GetAchievementInfoById(14);

        while (_isPlaying)
        {
            Vector3 distanceVector = _player.transform.position - tireHolder.transform.position;
            _passedDistance = WorldLoopController.Instance.GetRealPlayerZPosition() 
                            - tireHolder.transform.position.z;
            passedDistanceText.text = ((long) Mathf.Ceil(_passedDistance)).ToString() + " m.";

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
                GetJubileeAchievement(maxDistanceInKm, jubileeAchievement);
            }
            if (maxDistanceInM >= mapSizeFromPlayer)
                GetTimeFliesAchievement(1, timeFliesAchievement);
            GetFastAchievement((int) _playerRb.velocity.magnitude, fastAchievement);

            if (_isRewindEnabled)
                ApplyRewind();

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

    void ApplyRewind()
    {
        if (!_isRewindEnabled || _isPaused)
            return;
        _currentTimeScale = minRewindTimeScale
                          + MathfExtension.FuncSpeedApply(_playerRb.velocity.magnitude / maxRewindVelocity,
                                                          MathfExtension.FuncSpeed.SquareRoot)
                          * (maxRewindTimeScale - minRewindTimeScale);
        Time.timeScale = _currentTimeScale;
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

    void GetJubileeAchievement(int distanceInKm, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(distanceInKm, achievement);
        OnAchievementProgressChanged.Invoke(progress, 13);
    }

    void GetFastAchievement(int speed, AchievementInfo achievement)
    {
        var progress = new AchievementProgress(speed, achievement);
        OnAchievementProgressChanged.Invoke(progress, 14);
    }

    public void AfterDataLoaded(Database database) { }
}
