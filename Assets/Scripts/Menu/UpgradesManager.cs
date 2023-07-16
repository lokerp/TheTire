using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour, IAudioPlayable, IDataControllable
{
    public static UpgradesManager Instance { get; private set; }

    private enum LevelType
    {
        Bounciness,
        Power
    }

    public TextMeshProUGUI bouncinessLevelText;
    public TextMeshProUGUI powerLevelText;
    public TextMeshProUGUI bouncinessUpgradeCostText;
    public TextMeshProUGUI powerUpgradeCostText;

    public ButtonHolder bouncinessUpgradeButton;
    public ButtonHolder powerUpgradeButton;

    public Color noMoneyColor;

    public int maxBouncinessLevel;

    private int _bouncinessLevel;
    private int _powerLevel;

    private int _bouncinessUpgradeCost;
    private int _powerUpgradeCost;

    private Action OnBouncinessUpgradeButtonClick;
    private Action OnPowerUpgradeButtonClick;

    public static event Action<int, int> OnLevelUp;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        OnBouncinessUpgradeButtonClick = () => UpgradeLevel(LevelType.Bounciness);
        OnPowerUpgradeButtonClick = () => UpgradeLevel(LevelType.Power);
    }

    public void OnEnable()
    {
        bouncinessUpgradeButton.OnClick += OnBouncinessUpgradeButtonClick;
        powerUpgradeButton.OnClick += OnPowerUpgradeButtonClick;
    }

    public void OnDisable()
    {
        bouncinessUpgradeButton.OnClick -= OnBouncinessUpgradeButtonClick;
        powerUpgradeButton.OnClick -= OnPowerUpgradeButtonClick;
    }

    public void LoadData(Database database)
    {
        _bouncinessLevel = database.bouncinessLevel;
        _powerLevel = database.powerLevel;

        RefreshUpgradeCosts();
        RefreshLevelsInfo(database.currentMoney);
    }

    public void SaveData(ref Database database)
    {
        database.bouncinessLevel = _bouncinessLevel;
        database.powerLevel = _powerLevel;
    }

    private void UpgradeLevel(LevelType levelType)
    {
        int opCode = 0;
        switch (levelType)
        {
            case LevelType.Bounciness:
                opCode = MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount - _bouncinessUpgradeCost);
                if (opCode == 1)
                    _bouncinessLevel++;
                break;
            case LevelType.Power:
                opCode = MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount - _powerUpgradeCost);
                if (opCode == 1)
                    _powerLevel++;
                break;
        }
        if (opCode == 1)
        {
            AudioSources[0].Play();
            OnLevelUp?.Invoke(_bouncinessLevel, _powerLevel);
        }

        RefreshUpgradeCosts();
        RefreshLevelsInfo(MoneyManager.Instance.MoneyAmount);
    }

    private void RefreshLevelsInfo(int currentMoney)
    {
        bouncinessLevelText.text = _bouncinessLevel < maxBouncinessLevel ? _bouncinessLevel.ToString() : "Max";
        powerLevelText.text = _powerLevel.ToString();

        bouncinessUpgradeCostText.text = _bouncinessLevel < maxBouncinessLevel ? _bouncinessUpgradeCost.ToString() : "0";
        powerUpgradeCostText.text = _powerUpgradeCost.ToString();

        if (_bouncinessUpgradeCost > currentMoney || _bouncinessLevel >= maxBouncinessLevel)
        {
            bouncinessUpgradeButton.GetComponent<Image>().color = noMoneyColor;
            bouncinessUpgradeButton.enabled = false;
        }
        if (_powerUpgradeCost > currentMoney)
        {
            powerUpgradeButton.GetComponent<Image>().color = noMoneyColor;
            powerUpgradeButton.enabled = false;
        }
    }

    private void RefreshUpgradeCosts()
    {
        _bouncinessUpgradeCost =  100 * (_bouncinessLevel * (_bouncinessLevel + 1) / 2);
        _powerUpgradeCost = 100 * (_powerLevel * (_powerLevel + 1) / 2);
    }
}
