using AYellowpaper.SerializedCollections;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    private Animator _animator;
    [SerializedDictionary("Name", "Component")]
    [SerializeField] private SerializedDictionary<string, TextController> textControllers;
    [SerializedDictionary("Name", "Component")]
    [SerializeField] private SerializedDictionary<string, Image> imageControllers;

    private IEnumerator _openCoroutine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open(Action<Dictionary<string, TextController>, Dictionary<string, Image>> callback,
                     float timeOpenedInS, AudioSource sound)
    {
        callback?.Invoke(textControllers, imageControllers);
        if (_openCoroutine != null)
            StopCoroutine(_openCoroutine);
        _openCoroutine = Open(timeOpenedInS, sound);
        StartCoroutine(_openCoroutine);
    }

    private IEnumerator Open(float timeOpenedInS, AudioSource sound)
    {
        if (sound)
            PlaySound(sound);
        _animator.SetBool("IsNotificationOpen", true);
        yield return new WaitForSecondsRealtime(timeOpenedInS);
        _animator.SetBool("IsNotificationOpen", false);
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }
}
