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

    public ItemTypes selectedTire;
    public List<ItemTypes> availableTires;
}
