using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AchievementProgress
{
    public float progress;
    public bool isEarned;

    public AchievementProgress(float progress, bool isEarned)
    {
        this.progress = progress;
        this.isEarned = isEarned;
    }

    public AchievementProgress(float progress, AchievementInfo achievement)
    {
        this.progress = progress;
        if (progress >= achievement.totalProgress)
            isEarned = true;
        else
            isEarned = false;
    }


}
