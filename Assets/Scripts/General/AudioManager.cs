using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour, IDataControllable
{
    public enum VolumeType
    {
        Music,
        AllSounds,
        GameSounds
    }

    public static AudioManager Instance { get; private set; }
    public AudioMixer mixer;

    public AudioSource menuMusic;
    public AudioSource gameMusic;

    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundsVolumeSlider;
    private float musicVolume;
    private float soundsVolume;

    private UnityAction<float> listenerInstanceMusic;
    private UnityAction<float> listenerInstanceSounds;


    private void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        listenerInstanceMusic = (value) => ChangeVolume(VolumeType.Music, value / 10);
        listenerInstanceSounds = (value) => ChangeVolume(VolumeType.AllSounds, value / 10);
    }

    private void Start()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu")
            menuMusic.Play();
        else
            gameMusic.Play();
    }

    private void OnEnable()
    {
        if (_musicVolumeSlider != null)
            _musicVolumeSlider.onValueChanged.AddListener(listenerInstanceMusic);
        if (_soundsVolumeSlider != null)
            _soundsVolumeSlider.onValueChanged.AddListener(listenerInstanceSounds);
    }

    private void OnDisable()
    {
        if (_musicVolumeSlider != null)
            _musicVolumeSlider.onValueChanged.RemoveListener(listenerInstanceMusic);
        if (_soundsVolumeSlider != null)
            _soundsVolumeSlider.onValueChanged.RemoveListener(listenerInstanceSounds);
    }


    public void LoadData(Database database)
    {
        musicVolume = database.settings.currentMusicVolume;
        soundsVolume = database.settings.currentSoundsVolume;

        if (_musicVolumeSlider != null)
            _musicVolumeSlider.value = (int) (musicVolume * 10);
        if (_soundsVolumeSlider != null)
            _soundsVolumeSlider.value = (int) (soundsVolume * 10);
    }

    public void SaveData(ref Database database)
    {
        database.settings.currentMusicVolume = musicVolume;
        database.settings.currentSoundsVolume = soundsVolume;
    }

    public void ChangeVolume(VolumeType type, float value)
    {
        float newValueInDB = value != 0 ? Mathf.Clamp(Mathf.Log10(value) * 20, -80, 0) : -80;
        switch (type)
        {
            case VolumeType.Music:
                mixer.SetFloat("MusicVolume", newValueInDB);
                musicVolume = value;
                break;
            case VolumeType.AllSounds:
                mixer.SetFloat("SoundsVolume", newValueInDB);
                soundsVolume = value;
                break;
            case VolumeType.GameSounds:
                mixer.SetFloat("GameSoundsVolume", newValueInDB);
                break;
        }
    }
}
