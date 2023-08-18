using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsPage : MenuPage, IDataControllable
{
    [Serializable]
    public struct LanguageWithIcon
    {
        public TranslatableText language;
        public Sprite icon;
    }

    public static event Action OnSettingsAuth;
    public static event Action OnSettingsSaved;
    public static event Action<Settings> OnSettingsChanged;

    [Space, Header("Authorization")]
    [SerializeField] private Texture2D _unauthAvatar;
    [SerializeField] private RawImage _avatarHolder;
    [SerializeField] private TextController _nameHolder;
    [SerializeField] private Button _authButton;

    [Space, Header("Language")]
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private List<LanguageWithIcon> _languages;

    [Space, Header("Music")]
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundsVolumeSlider;

    [Space]
    [SerializeField] private Button _saveButton;

    private Settings _settings;
    private Settings _savedSettings;
    private UnityAction<float> _listenerInstanceMusic;
    private UnityAction<float> _listenerInstanceSounds;

    protected override void Awake()
    {
        base.Awake();

        _saveButton.interactable = false;

        _listenerInstanceMusic = (value) =>
        {
            CheckVolumeChange(VolumeTypes.Music, value / 10);
        };
        _listenerInstanceSounds = (value) =>
        {
            CheckVolumeChange(VolumeTypes.Sounds, value / 10);
        };

        _languageDropdown.options = new(new TMP_Dropdown.OptionData[_languages.Count]);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _saveButton.onClick.AddListener(SaveSettings);
        _languageDropdown.onValueChanged.AddListener(CheckLanguageChange);
        _musicVolumeSlider.onValueChanged.AddListener(_listenerInstanceMusic);
        _soundsVolumeSlider.onValueChanged.AddListener(_listenerInstanceSounds);
        _authButton.onClick.AddListener(OnAuthButtonClickHandler);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _saveButton.onClick.RemoveListener(SaveSettings);
        _languageDropdown.onValueChanged.RemoveListener(CheckLanguageChange);
        _musicVolumeSlider.onValueChanged.RemoveListener(_listenerInstanceMusic);
        _soundsVolumeSlider.onValueChanged.RemoveListener(_listenerInstanceSounds);
        _authButton.onClick.RemoveListener(OnAuthButtonClickHandler);
    }

    public override void Close()
    {
        base.Close();

        _settings = _savedSettings;
        RefreshSettingsValues();
    }

    private void OnSettingsChangeInvoke()
    {
        CanBeSavedCheck();
        OnSettingsChanged?.Invoke(_settings);
    }

    private void CanBeSavedCheck()
    {
        if (_settings != _savedSettings)
            _saveButton.interactable = true;
        else
            _saveButton.interactable = false;
    }

    void SaveSettings()
    {
        _savedSettings = _settings;
        CanBeSavedCheck();
        OnSettingsSaved?.Invoke();
    }

    void CheckLanguageChange(int value)
    {
        switch (value)
        {
            case 0:
                _settings.language = Languages.English;
                break;
            case 1:
                _settings.language = Languages.Russian;
                break;
        
        }
        RefreshDropdownValues();

        OnSettingsChangeInvoke();
    }

    void CheckVolumeChange(VolumeTypes volumeType, float value)
    {
        switch (volumeType)
        {
            case VolumeTypes.Music:
                _settings.musicVolume = value;
                break;
            case VolumeTypes.Sounds:
                _settings.soundsVolume = value;
                break;
        }

        OnSettingsChangeInvoke();
    }

    public void SaveData(ref Database database)
    {
        database.settings = _savedSettings;
    }

    public void LoadData(Database database)
    {
        _savedSettings = database.settings;
        _settings = _savedSettings;
        RefreshSettingsValues();
        RefreshPlayerInfo();

        RefreshDropdownValues();
    }

    private void RefreshDropdownValues()
    {
        for (int i = 0; i < _languages.Count; i++)
        {
            _languageDropdown.options[i] = new TMP_Dropdown.OptionData()
            {
                text = _languages[i].language.GetText(_settings.language),
                image = _languages[i].icon
            };
        }
        _languageDropdown.RefreshShownValue();
    }

    private void RefreshSettingsValues()
    {
        _languageDropdown.value = (int) (_settings.language);
        _musicVolumeSlider.value = (int) (_settings.musicVolume * 10);
        _soundsVolumeSlider.value = (int) (_settings.soundsVolume * 10);
    }

    private async void RefreshPlayerInfo()
    {
        try
        {
            PlayerInfo playerInfo = await DataManager.Instance.GetPlayerInfoAsync();
            _nameHolder.gameObject.SetActive(true);
            _nameHolder.Text = new TranslatableText { English = playerInfo.name, Russian = playerInfo.name };
            _avatarHolder.texture = playerInfo.avatar;
            _authButton.gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            _nameHolder.gameObject.SetActive(false);
            _avatarHolder.texture = _unauthAvatar;
            _authButton.gameObject.SetActive(true);
        }
    }

    private async void OnAuthButtonClickHandler()
    {
        if (!APIBridge.IsPlayerAuth())
        {
            await APIBridge.Instance.AuthAsync();
            if (APIBridge.IsPlayerAuth())
                OnSettingsAuth?.Invoke();
        }
        else
        {
            await APIBridge.Instance.RequestPlayerPermissionAsync();
            RefreshPlayerInfo();
        }
    }

    public void AfterDataLoaded(Database database) { }
}
