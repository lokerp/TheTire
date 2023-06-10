using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour, IDataControllable
{
    enum VolumeType
    {
        Music,
        Sounds
    }

    public static AudioManager Instance { get; private set; }
    public AudioMixer mixer;
    public Slider musicVolumeSlider;
    public Slider soundsVolumeSlider;
    private int musicVolume;
    private int soundsVolume;

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

        listenerInstanceMusic = (value) => ChangeVolume(VolumeType.Music, (int) value);
        listenerInstanceSounds = (value) => ChangeVolume(VolumeType.Sounds, (int) value);
    }

    private void OnEnable()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(listenerInstanceMusic);
        if (soundsVolumeSlider != null)
            soundsVolumeSlider.onValueChanged.AddListener(listenerInstanceSounds);
    }

    private void OnDisable()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(listenerInstanceMusic);
        if (soundsVolumeSlider != null)
            soundsVolumeSlider.onValueChanged.RemoveListener(listenerInstanceSounds);
    }


    public void LoadData(Database database)
    {
        musicVolume = database.settings.currentMusicVolume;
        soundsVolume = database.settings.currentSoundsVolume;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        if (soundsVolumeSlider != null)
            soundsVolumeSlider.value = soundsVolume;
    }

    public void SaveData(ref Database database)
    {
        database.settings.currentMusicVolume = musicVolume;
        database.settings.currentSoundsVolume = soundsVolume;
    }

    private void ChangeVolume(VolumeType type, int value)
    {
        float newValueInDB = value != 0 ? Mathf.Clamp(Mathf.Log10((float) value / 10) * 20, -80, 0) : -80;
        switch (type)
        {
            case VolumeType.Music:
                mixer.SetFloat("MusicVolume", newValueInDB);
                musicVolume = value;
                break;
            case VolumeType.Sounds:
                mixer.SetFloat("SoundsVolume", newValueInDB);
                soundsVolume = value;
                break;
        }
    }
}
