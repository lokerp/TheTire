using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Database
{
    public Languages currentLanguage;
    public int currentLaunches;
    public int currentMoney;

    public ItemInfo currentTire;
    public List<ItemInfo> availableTires;
}
