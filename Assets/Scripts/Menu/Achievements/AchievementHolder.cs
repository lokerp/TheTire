using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHolder : MonoBehaviour
{
    AchievementInfo info;
    AchievementProgress progress;
    [SerializeField] private Color notEarnedAlpha;
    [SerializeField] private Color earnedAlpha;
    [SerializeField] private Image icon;
    [SerializeField] private TextController title;

    private void Awake()
    {
        info = null;
    }

    public void SetInfo((AchievementInfo, AchievementProgress) i)
    {
        info = i.Item1;
        progress = i.Item2;

        TextMeshProUGUI titleContr = title.GetComponent<TextMeshProUGUI>();

        icon.color = notEarnedAlpha;
        titleContr.color = notEarnedAlpha;

        if (info.isSecret && !progress.isEarned)
        {
            icon.sprite = Resources.Load<Sprite>(info.secretImagePath);
            title.text = info.secretTitle;
        }
        else
        {
            icon.sprite = Resources.Load<Sprite>(info.imagePath);
            title.text = info.title;
            if (progress.isEarned)
            {
                icon.color = earnedAlpha;
                titleContr.color = earnedAlpha;
            }
        }

        title.RefreshText();
    }

    public (AchievementInfo, AchievementProgress) GetInfo()
    {
        return (info, progress);
    }
}
