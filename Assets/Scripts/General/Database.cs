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

    public ItemTypes selectedTire;
    public List<ItemTypes> availableTires;

    public ItemTypes selectedWeapon;
    public List<ItemTypes> availableWeapons;

    public Records records;
}
