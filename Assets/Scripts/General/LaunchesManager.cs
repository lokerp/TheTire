using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchesManager : MonoBehaviour, IDataControllable
{
    public static LaunchesManager Instance { get; private set; }
    public int LaunchesAmount { get; private set; }
    public int maxLaunches = 10;
    public float timeToRecoverInS = 5;
    public TextMeshProUGUI launchesText;
    public Color launchesErrorColor;
    public Animator timePanel;
    public TextMeshProUGUI timeText;

    private int _timePassedInS = 0;
    private Color _launchesDefaultColor;

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

        if (launchesText != null)
            _launchesDefaultColor = launchesText.color;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
            StartCoroutine(Timer());
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

    private IEnumerator Timer()
    {
        while (true)
        {
            if (LaunchesAmount >= maxLaunches)
            {
                yield return null;
                continue;
            }

            else if (_timePassedInS < timeToRecoverInS)
            {
                SetTimeText();
                _timePassedInS++;
            }

            else if (_timePassedInS >= timeToRecoverInS)
            {
                _timePassedInS = 0;
                LaunchesAmount++;
                SetTimeText();
                RefreshLaunchesText();
            }

            yield return new WaitForSecondsRealtime(1);
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

        int minutes = (int) ((timeToRecoverInS - _timePassedInS) / 60);
        int seconds = (int) ((timeToRecoverInS - _timePassedInS) % 60);

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

    public IEnumerator ShowError()
    {
        StartCoroutine(OpenTimePanel());
        
        for (int i = 0; i < 3; i++)
        {
            launchesText.color = launchesErrorColor;
            yield return new WaitForSecondsRealtime(0.2f);
            launchesText.color = _launchesDefaultColor;
            yield return new WaitForSecondsRealtime(0.2f);
        }

        yield break;
    }

    public void ReduceLaunchesAmount()
    {
        LaunchesAmount--;
    }
}
