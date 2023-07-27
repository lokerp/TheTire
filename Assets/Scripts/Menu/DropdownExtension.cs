using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropdownExtension : MonoBehaviour, IAudioPlayable
{
    public TMP_Dropdown dropdown;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private UnityAction<int> onValueChangedHandler;

    void Awake()
    {
        onValueChangedHandler = (int _) => PlaySound(AudioSources[1]);
    }

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
        dropdown.onValueChanged.AddListener(onValueChangedHandler);
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
        dropdown.onValueChanged.RemoveListener(onValueChangedHandler);
    }

    void UIClickHandler(GameObject gameObject)
    {
        if (gameObject == dropdown.gameObject)
            PlaySound(AudioSources[0]);
    }

    public void PlaySound(AudioSource source)
    {
        source.Stop();
        source.Play();
    }
}
