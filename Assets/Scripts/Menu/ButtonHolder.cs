using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHolder : MonoBehaviour, IAudioPlayable
{
    public GameObject button;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void OnEnable()
    {
        UIEvents.OnUIClick += UIClickHandler;
    }

    private void OnDisable()
    {
        UIEvents.OnUIClick -= UIClickHandler;
    }

    void UIClickHandler(GameObject gameObject)
    {
        if (gameObject == button && AudioSources.Count != 0)
            PlaySound(AudioSources[0]);
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }
}
