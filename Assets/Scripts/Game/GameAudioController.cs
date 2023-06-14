using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudioController : MonoBehaviour, IAudioPlayable
{
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
    public float windDefaultPitch = 1;
    [Range(0, 5)] public float windPitchRange;

    [field: SerializeField, Space]
    public List<AudioSource> AudioSources { get; private set; }
    public float peakSoundsVelocityVolume;
    [Space]
    [Range(0, 1)] public float minWaterJumpVolume;
    [Range(0, 1)] public float maxWaterJumpVolume;
    [Space]
    [Range(0, 1)] public float minCliffsJumpVolume;
    [Range(0, 1)] public float maxCliffsJumpVolume;
    [Range(0, 2)] public float minCliffsJumpPitch;
    [Range(0, 2)] public float maxCliffsJumpPitch;


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
        float velocityPitchCoef = Mathf.Clamp01(YZVelocity / windPeakVelocityPitch) * windPitchRange;
        float heightPitchCoef = Mathf.Clamp(1 - Mathf.Abs(playerHeight - windPeakHeightPitch) / windHeightPitchRadius,
                                            1 - windHeightPitchWeight,
                                            1);
        windSource.volume = Mathf.Lerp(windSource.volume, heightVolumeCoef + velocityVolumeCoef, Time.deltaTime * 5);
        windSource.pitch = Mathf.Lerp(windSource.pitch, windDefaultPitch + velocityPitchCoef * heightPitchCoef, Time.deltaTime * 5);

    }

    void PlayCollisionSounds(Collision collision)
    {
        float volume = 1;
        float pitch = 1;
        float velocityCoef = Mathf.Clamp01(collision.relativeVelocity.magnitude / peakSoundsVelocityVolume);
        Vector3 collisionPoint = collision.GetContact(0).point;
        int soundIndex = -1;

        if (collision.gameObject.CompareTag("Water"))
        {
            volume = minWaterJumpVolume + velocityCoef * (maxWaterJumpVolume - minWaterJumpVolume);
            soundIndex = 0;
        }
        else if (collision.gameObject.CompareTag("Cliffs"))
        {
            volume = minCliffsJumpVolume + velocityCoef * (maxCliffsJumpVolume - minCliffsJumpVolume);
            pitch = minCliffsJumpPitch + (1 - velocityCoef) * (maxCliffsJumpPitch - minCliffsJumpPitch);
            soundIndex = 1;
        }


        if (soundIndex == -1)
            return;
        AudioSources[soundIndex].volume = volume;
        AudioSources[soundIndex].pitch = pitch;
        PlaySound(AudioSources[soundIndex], collisionPoint);
    }

    public void PlaySound(AudioSource source, Vector3 position)
    {
        source.PlayAtPosition(position);
    }
}
