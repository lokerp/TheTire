using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Records
{
    public Dictionary<byte, AchievementProgress> AchievementProgress { get; set; }
    public float RecordDistance { get; set; }
    public int AchievementsEarnedCount { get; set; }

    public Records()
    {
        AchievementProgress = null;
        RecordDistance = 0f;
        AchievementsEarnedCount = 0;
    }


}
