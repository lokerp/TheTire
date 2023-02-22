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

    public TypesUtility.Item.Tire.Type currentTire;
    public List<TypesUtility.Item.Tire.Type> availableTires;
}
