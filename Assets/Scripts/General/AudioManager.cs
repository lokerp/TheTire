using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : StonUndestroyable<AudioManager>, IDataControllable
{
    public AudioMixer mixer;

    public AudioSource menuMusic;
    public AudioSource gameMusic;

    private void OnEnable()
    {
        SettingsPage.OnSettingsChanged += ChangeVolume;
        SceneManager.activeSceneChanged += PlayMusic;
        APIBridge.OnAdvertisementOpen += () => MuteGame(true);
        APIBridge.OnAdvertisementClose += (_) => MuteGame(false);
    }

    private void PlayMusic(Scene pastScene, Scene newScene)
    {
        MuteGame(false);

        if (newScene.name == "Menu")
        {
            gameMusic.Stop();
            menuMusic.Play();
        }
        else if (newScene.name == "Game")
        {
            menuMusic.Stop();
            gameMusic.Play();
        }
    }

    public void LoadData(Database database)
    {
        ChangeVolume(database.settings);
    }

    public void SaveData(ref Database database) { }

    private float Float01ToDb(float value)
    {
        return value != 0 ? Mathf.Clamp(Mathf.Log10(value) * 20, -80, 0) : -80;
    }

    private void ChangeVolume(Settings settings)
    {
        float musicVolumeInDb = Float01ToDb(settings.musicVolume);
        float soundsVolumeInDb = Float01ToDb(settings.soundsVolume);

        mixer.SetFloat("MusicVolume", musicVolumeInDb);
        mixer.SetFloat("SoundsVolume", soundsVolumeInDb);
    }

    public void ChangeVolume(VolumeTypes volumeType, float value)
    {
        var volumeInDb = Float01ToDb(value);
        switch (volumeType)
        {
            case VolumeTypes.Music:
                mixer.SetFloat("MusicVolume", volumeInDb);
                break;
            case VolumeTypes.Sounds:
                mixer.SetFloat("SoundsVolume", volumeInDb);
                break;
            case VolumeTypes.GameSounds:
                mixer.SetFloat("GameSoundsVolume", volumeInDb);
                break;
            case VolumeTypes.Master:
                mixer.SetFloat("MasterVolume", volumeInDb);
                break;
        }
    }

    public void AfterDataLoaded(Database database) { }

    public void MuteGame(bool flag)
    {
        if (flag)
            ChangeVolume(VolumeTypes.Master, 0);
        else
            ChangeVolume(VolumeTypes.Master, 1);

    }
}
