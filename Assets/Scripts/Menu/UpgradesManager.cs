using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour, IAudioPlayable, IDataControllable
{
    private enum LevelType
    {
        Bounciness,
        Power
    }

    public int maxHandlingLevel;
    public int maxPowerLevel;

    public TextMeshProUGUI bouncinessLevelText;
    public TextMeshProUGUI powerLevelText;
    public TextMeshProUGUI bouncinessUpgradeCostText;
    public TextMeshProUGUI powerUpgradeCostText;

    public ButtonHolder bouncinessUpgradeButton;
    public ButtonHolder powerUpgradeButton;

    public Color noMoneyColor;

    private int _bouncinessLevel;
    private int _powerLevel;

    private int _bouncinessUpgradeCost;
    private int _powerUpgradeCost;

    private Action OnBouncinessUpgradeButtonClick;
    private Action OnPowerUpgradeButtonClick;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    public void Awake()
    {
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
        _bouncinessLevel = database.handlingLevel;
        _powerLevel = database.powerLevel;

        RefreshUpgradeCosts();
        RefreshLevelsInfo();
    }

    public void SaveData(ref Database database)
    {
        database.handlingLevel = _bouncinessLevel;
        database.powerLevel = _powerLevel;
    }

    private void UpgradeLevel(LevelType levelType)
    {
        int opCode = 0;
        switch (levelType)
        {
            case LevelType.Bounciness:
                if (_bouncinessLevel >= maxHandlingLevel)
                    throw new System.Exception("Trying to upgrade level while its' max has been reached!");
                opCode = MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount - _bouncinessUpgradeCost);
                if (opCode == 1)
                    _bouncinessLevel++;
                break;
            case LevelType.Power:
                if (_powerLevel >= maxPowerLevel)
                    throw new System.Exception("Trying to upgrade level while its' max has been reached!");
                opCode = MoneyManager.Instance.ChangeMoneyAmount(MoneyManager.Instance.MoneyAmount - _powerUpgradeCost);
                if (opCode == 1)
                    _powerLevel++;
                break;
        }
        if (opCode == 1)
            AudioSources[0].Play();

        RefreshUpgradeCosts();
        RefreshLevelsInfo();
    }

    private void RefreshLevelsInfo()
    {
        bouncinessLevelText.text = _bouncinessLevel.ToString() + " / " + maxHandlingLevel.ToString();
        powerLevelText.text = _powerLevel.ToString() + " / " + maxPowerLevel.ToString();

        bouncinessUpgradeCostText.text = _bouncinessUpgradeCost.ToString();
        powerUpgradeCostText.text = _powerUpgradeCost.ToString();

        if (_bouncinessUpgradeCost > MoneyManager.Instance.MoneyAmount || _bouncinessLevel >= maxHandlingLevel)
        {
            bouncinessUpgradeButton.GetComponent<Image>().color = noMoneyColor;
            bouncinessUpgradeButton.enabled = false;
        }
        if (_powerUpgradeCost > MoneyManager.Instance.MoneyAmount || _powerLevel >= maxPowerLevel)
        {
            powerUpgradeButton.GetComponent<Image>().color = noMoneyColor;
            powerUpgradeButton.enabled = false;
        }
    }

    private void RefreshUpgradeCosts()
    {
        if (_bouncinessLevel >= maxHandlingLevel)
            _bouncinessUpgradeCost = 0;
        if (_powerLevel >= maxPowerLevel)
        {
            _powerLevel = 0;
            return;
        }

        _bouncinessUpgradeCost =  200 * _bouncinessLevel * (_bouncinessLevel + 1);
        _powerUpgradeCost = 200 * _powerLevel * (_powerLevel + 1);
    }
}
