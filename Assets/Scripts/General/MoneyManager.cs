using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : StonUndestroyable<MoneyManager>, IDataControllable
{
    public static event Action<int> OnMoneyChanged;
    public int MoneyAmount { get; private set; }
    public int maxMoneyAmount = 9999999;

    private void OnEnable()
    {
        AchievementsManager.OnAchievementEarned += GetMoneyForAchievement;
    }

    private void OnDisable()
    {
        AchievementsManager.OnAchievementEarned -= GetMoneyForAchievement;
    }

    public void SaveData(ref Database database)
    {
        database.currentMoney = MoneyAmount;
    }

    public void LoadData(Database database)
    {
        ChangeMoneyAmount(database.currentMoney);
    }

    public int ChangeMoneyAmount(int amount)
    {
        if (amount < 0)
            return 0;
        else if (amount >= maxMoneyAmount)
            MoneyAmount = maxMoneyAmount;
        else
            MoneyAmount = amount;
        OnMoneyChanged?.Invoke(MoneyAmount);
        return 1;
    }

    public void GetMoneyForAchievement(AchievementInfo achievement)
    {
        ChangeMoneyAmount(MoneyAmount + achievement.moneyPrize);
    }

    public void AfterDataLoaded(Database database) { }
}
