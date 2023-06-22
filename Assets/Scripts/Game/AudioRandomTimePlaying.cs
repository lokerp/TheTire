using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomTimePlaying : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _audioSource.time = Random.Range(0f, 1f) * _audioSource.clip.length;
    }
}
