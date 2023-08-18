using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Database
{
    public Settings settings;

    public int currentLaunches;
    public int currentMoney;

    public int bouncinessLevel;
    public int powerLevel;

    public int totalLaunchesCount;
    public int adWatchedCount;
    public int redZoneThrowsCount;
    public DateTime lastSession = DateTime.UtcNow;

    public ItemTypes selectedTire;
    public List<ItemTypes> availableTires;

    public ItemTypes selectedWeapon;
    public List<ItemTypes> availableWeapons;

    public Records records;
}

[Serializable]
public struct Settings : IEquatable<Settings>
{
    public Languages language;
    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float soundsVolume;

    public bool Equals(Settings other)
    {
        return this.language == other.language
            && Mathf.Approximately(this.musicVolume, other.musicVolume)
            && Mathf.Approximately(this.soundsVolume, other.soundsVolume);
    }

    public override bool Equals(object obj)
    {
        return obj is Settings other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(language, musicVolume, soundsVolume);
    }

    public static bool operator ==(Settings s1, Settings s2)
    {
        return s1.Equals(s2);
    }

    public static bool operator !=(Settings s1, Settings s2)
    {
        return !(s1 == s2);
    }
}

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
