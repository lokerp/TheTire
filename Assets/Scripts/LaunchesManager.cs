using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchesManager : MonoBehaviour, IDataControllable
{
    public static LaunchesManager Instance { get; private set; }
    public int LaunchesAmount { get; private set; }
    public int maxLaunches = 10;
    public int timeToRecoverInMin = 5;
    public TextMeshProUGUI launchesText;
    public Animator timePanel;
    public TextMeshProUGUI timeText;
    private bool _isTimerRunning;

    private int timeToRecoverInSec;
    private int _timePassed = 0;

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        timeToRecoverInSec = 60 * timeToRecoverInMin;
    }

    void Update()
    {
        if (!_isTimerRunning && LaunchesAmount < maxLaunches)
        {
            _timePassed = 0;
            _isTimerRunning = true;
            StartCoroutine(StartTimer());
        }
    }

    public void LoadData(Database database)
    {
        LaunchesAmount = database.currentLaunches;
        RefreshLaunchesText();
        SetTimeText();
    }

    public void SaveData(ref Database database)
    {
        database.currentLaunches = LaunchesAmount;
    }

    private IEnumerator StartTimer()
    {
        while(_timePassed < timeToRecoverInSec)
        {
            SetTimeText();
            _timePassed++;
            yield return new WaitForSecondsRealtime(1);
        }

        if (_timePassed >= timeToRecoverInSec)
        {
            _isTimerRunning = false;
            LaunchesAmount++;
            SetTimeText();
            RefreshLaunchesText();
            DataManager.Instance.SaveGame();
        }
    }

    void SetTimeText()
    {
        if (timeText == null)
            return;

        if (LaunchesAmount == maxLaunches)
        {
            timeText.text = $"0:00";
            return;
        }
        int minutes = (timeToRecoverInSec - _timePassed) / 60;
        int seconds = (timeToRecoverInSec - _timePassed) % 60;

        timeText.text = string.Format("{0}:{1:d2}", minutes, seconds);
    }

    void UIClickHandler(GameObject gameObject)
    {
        switch (gameObject.tag)
        {
            case "TimePanelButton":
                StartCoroutine(OpenTimePanel());
                break;
            case "AdButton":
                break;
        }
    }

    private IEnumerator OpenTimePanel()
    {
        if (!timePanel.GetBool("IsOpen"))
        {
            timePanel.SetBool("IsOpen", true);
            yield return new WaitForSecondsRealtime(5);
            timePanel.SetBool("IsOpen", false);
        }
    }

    void RefreshLaunchesText()
    {
        if (launchesText != null)
            launchesText.text = $"{LaunchesAmount}/{maxLaunches}";
    }

    public bool CanPlay()
    {
        if (LaunchesAmount > 0)
            return true;
        return false;
    }

    public void ReduceLaunchesAmount()
    {
        LaunchesAmount--;
    }
}
