using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDescription : MonoBehaviour
{
    CanvasGroup canvGroup;
    public Image icon;
    public TextController title;
    public TextController description;
    public TextMeshProUGUI progress;
    public Color earnedProgressColor;
    public Color notEarnedProgressColor;
    [Range(0, 1)] public float notEarnedAlpha;

    private void Awake()
    {
        canvGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        Close();
    }


    public void Show((AchievementInfo, AchievementProgress) i)
    {
        AchievementInfo info = i.Item1;
        AchievementProgress pr = i.Item2;

        canvGroup.alpha = 1;

        if (info.isSecret && !pr.isEarned)
        {
            title.text = info.secretTitle;
            description.text = info.secretDescription;
            icon.sprite = Resources.Load<Sprite>(info.secretImagePath);
            progress.text = 0 + " / " + 0;
            progress.color = notEarnedProgressColor;
            canvGroup.alpha = notEarnedAlpha;
        }
        else
        {
            title.text = info.title;
            description.text = info.description;
            icon.sprite = Resources.Load<Sprite>(info.imagePath);
            progress.text = (int) pr.progress + " / " + info.totalProgress;

            if (pr.isEarned)
                progress.color = earnedProgressColor;
            else
            {
                progress.color = notEarnedProgressColor;
                canvGroup.alpha = notEarnedAlpha;
            }
        }

        title.RefreshText();
        description.RefreshText();
    }

    public void Close()
    {
        canvGroup.alpha = 0;
    }
}