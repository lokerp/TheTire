using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudioController : MonoBehaviour
{
    [Serializable]
    public struct AudioParameters
    {
        public AudioSource source;
        [Range(0, 1)] public float minVolume;
        [Range(0, 1)] public float maxVolume;
        [Range(0, 2)] public float minPitch;
        [Range(0, 2)] public float maxPitch;
    }

    public static GameAudioController Instance { get; private set; }

    public AudioMixer mixer;
    [Space]

    public AudioSource windSource;
    public float windPeakHeightVolume;
    [Range(0, 1)] public float windHeightVolumeWeight;
    public float windPeakVelocityVolume;
    [Range(0, 1)] public float windVelocityVolumeWeight;
    public float windPeakVelocityPitch;
    public float windPeakHeightPitch;
    public float windHeightPitchRadius;
    [Range(0, 1)] public float windHeightPitchWeight;
    [Range(0, 5)] public float windMinPitch = 1;
    [Range(0, 5)] public float windMaxPitch;

    [Space]
    public float peakSoundsVelocityVolume;
    [field: SerializeField]
    public List<AudioParameters> AudioSources { get; private set; }

    private PlayerController _playerController;

    private void OnDisable()
    {
        _playerController.OnCollision -= PlayCollisionSounds;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        windSource.Play();
        windSource.volume = 0;
        _playerController = PlayerController.Instance;
        _playerController.OnCollision += PlayCollisionSounds;
    }

    void Update()
    {
        ControlWindSounds();
    }

    void ControlWindSounds()
    {
        float playerHeight = Mathf.Clamp(_playerController.transform.position.y, 0, float.MaxValue);
        float heightVolumeCoef = Mathf.Clamp01(playerHeight / windPeakHeightVolume) * windHeightVolumeWeight;
        float YZVelocity = Vector3.ProjectOnPlane(_playerController.Rigidbody.velocity, Vector3.right).magnitude;
        float velocityVolumeCoef = Mathf.Clamp01(YZVelocity / windPeakVelocityVolume) * windVelocityVolumeWeight;
        float velocityPitchCoef = Mathf.Clamp01(YZVelocity / windPeakVelocityPitch);
        float heightPitchCoef = Mathf.Clamp(1 - Mathf.Abs(playerHeight - windPeakHeightPitch) / windHeightPitchRadius,
                                            1 - windHeightPitchWeight,
                                            1);
        windSource.volume = Mathf.Lerp(windSource.volume, heightVolumeCoef + velocityVolumeCoef, Time.deltaTime * 5);
        windSource.pitch = Mathf.Lerp(windSource.pitch, 
                                      windMinPitch + velocityPitchCoef * heightPitchCoef * (windMaxPitch - windMinPitch),
                                      Time.deltaTime * 5);

    }

    void PlayCollisionSounds(GameObject collider, Vector3 collisionPoint, Vector3 collisionVelocity)
    {
        AudioParameters audio = AudioSources.Find((x) => x.source.CompareTag(collider.tag));
        if (audio.source == null)
            return;
        float velocityCoef = Mathf.Clamp01(collisionVelocity.magnitude / peakSoundsVelocityVolume);

        audio.source.volume = audio.minVolume + velocityCoef * (audio.maxVolume - audio.minVolume);
        audio.source.pitch = audio.minPitch + (1 - velocityCoef) * (audio.minPitch - audio.minPitch);
        PlaySound(audio.source, collisionPoint);
    }

    public void PlaySound(AudioSource source, Vector3 position)
    {
        source.PlayAtPosition(position);
    }
}
