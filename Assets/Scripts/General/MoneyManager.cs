using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour, IDataControllable
{
    public static MoneyManager Instance { get; private set; }
    public int MoneyAmount { get; private set; }

    public int maxMoneyAmount = 99999;
    public TextMeshProUGUI moneyText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }

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
        MoneyAmount = database.currentMoney;
        RefreshMoneyText();
    }

    public int ChangeMoneyAmount(int amount)
    {
        if (amount < 0)
            return 0;
        else if (amount >= maxMoneyAmount)
            MoneyAmount = maxMoneyAmount;
        else
            MoneyAmount = amount;
        RefreshMoneyText();
        return 1;
    }

    public void GetMoneyForAchievement(AchievementInfo achievement)
    {
        ChangeMoneyAmount(MoneyAmount + achievement.moneyPrize);
    }

    void RefreshMoneyText()
    {
        if (moneyText != null)
            moneyText.text = MoneyAmount.ToString();
    }
}
