using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DropdownExtension : MonoBehaviour, IAudioPlayable
{
    public TMP_Dropdown dropdown;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
        dropdown.onValueChanged.AddListener((int _) => PlaySound(AudioSources[1]));
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
        dropdown.onValueChanged.RemoveListener((int _) => PlaySound(AudioSources[0]));
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
