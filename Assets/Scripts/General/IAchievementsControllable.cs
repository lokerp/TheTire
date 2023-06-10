using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAchievementsControllable
{
    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }
}
