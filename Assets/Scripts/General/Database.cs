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

    public int launchesCount;
    public int adWatchedCount;
    public int redZoneThrowsCount;
    public DateTime lastSession = DateTime.UtcNow;
    [NonSerialized] public bool isFirstVisitPerSession = false;

    public ItemTypes selectedTire;
    public List<ItemTypes> availableTires;

    public ItemTypes selectedWeapon;
    public List<ItemTypes> availableWeapons;

    public Records records;
}
