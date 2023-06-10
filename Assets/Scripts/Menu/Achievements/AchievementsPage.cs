using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementsPage : MonoBehaviour
{
    public List<CanvasGroup> pages;
    public TextMeshProUGUI recordDistanceHolder;
    public TextMeshProUGUI achievementsEarnedCountHolder;
    public List<AchievementHolder> achievementHolders;
    public AchievementDescription achievementDescription;
    public GameObject leftArrow;
    public GameObject rightArrow;

    private int currentPageIndex;


    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
        AchievementsManager.OnAchievementsLoad += RefreshAchievements;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
        AchievementsManager.OnAchievementsLoad -= RefreshAchievements;
    }

    private void Awake()
    {
        currentPageIndex = 0;
        OpenPage();
    }

    private void Start()
    {
        achievementDescription.Close();
        recordDistanceHolder.text = ((int)AchievementsManager.Instance.Records.RecordDistance).ToString();
        achievementsEarnedCountHolder.text = AchievementsManager.Instance.Records.AchievementsEarnedCount.ToString()
                                             + "/" + AchievementsManager.Instance.GetAchievementsList().Count;
    }

    void UIClickHandler(GameObject gameObject)
    {
        AchievementHolder clickedAchievement = achievementHolders.Find((item) => 
                                               item.GetComponent<ButtonHolder>().button == gameObject);
        if (clickedAchievement != null)
            achievementDescription.Show(clickedAchievement.GetInfo());
        else
            achievementDescription.Close();

        if (gameObject == leftArrow.GetComponent<ButtonHolder>().button)
        {
            currentPageIndex--;
            OpenPage();
        }
        else if (gameObject == rightArrow.GetComponent<ButtonHolder>().button)
        {
            currentPageIndex++;
            OpenPage();
        }
    }

    void OpenPage()
    {
        bool hasOpened = false;

        for (int i = 0; i < pages.Count; i++)
        {
            if (i == currentPageIndex)
            {
                pages[i].alpha = 1;
                pages[i].blocksRaycasts = true;
                hasOpened = true;
            }
            else
            {
                pages[i].alpha = 0;
                pages[i].blocksRaycasts = false;
            }
        }

        if (currentPageIndex == 0) leftArrow.SetActive(false);
        else leftArrow.SetActive(true);
        if (currentPageIndex == pages.Count - 1) rightArrow.SetActive(false);
        else rightArrow.SetActive(true);

        if (!hasOpened)
            throw new System.Exception("Error! Achievements Page hasn't been opened!");
    }

    public void RefreshAchievements(Dictionary<byte, AchievementProgress> achievementProgress)
    {
        achievementProgress = achievementProgress.OrderByDescending(x => x.Value.isEarned)
                                                 .ThenBy(x => AchievementsManager.Instance.GetAchievementInfoById(x.Key).isSecret)
                                                 .ToDictionary(pair => pair.Key, pair => pair.Value);
        if (achievementHolders.Count < achievementProgress.Count)
            throw new System.Exception("achievementHolders.Count < achievements.Count !!!");
        int i = 0;
        foreach (var el in achievementProgress)
        {
            if (i >= achievementHolders.Count)
                break;
            var achievement = AchievementsManager.Instance.GetAchievementInfoById(el.Key);
            var progress = el.Value;
            var canv = achievementHolders[i].GetComponent<CanvasGroup>();
            canv.alpha = 1;
            canv.blocksRaycasts = true;
            achievementHolders[i].SetInfo((achievement, progress));
            i++;
        }
        for (; i < achievementHolders.Count; i++)
        {
            var canv = achievementHolders[i].GetComponent<CanvasGroup>();
            canv.alpha = 0;
            canv.blocksRaycasts = false;
        }
    }
}
