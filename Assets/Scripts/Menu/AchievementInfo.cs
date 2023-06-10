using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/AchievementInfo", order = 1)]
public class AchievementInfo : ScriptableObject
{
    public TranslatableText title;
    public TranslatableText secretTitle;
    public TranslatableText description;
    public TranslatableText secretDescription;
    public int moneyPrize;
    public int totalProgress;
    public bool isSecret;
    public string imagePath;
    public string secretImagePath;
    public byte id;

    AchievementInfo()
    {
        secretTitle = new TranslatableText()
        {
            English = "Secret",
            Russian = "Секретно"
        };

        secretDescription = new TranslatableText()
        {
            English = "Keep playing and you'll get it... maybe",
            Russian = "Играй и ты получишь его... возможно"
        };

        secretImagePath = "AchievementsIcons/SecretAchievement";
    }
}
