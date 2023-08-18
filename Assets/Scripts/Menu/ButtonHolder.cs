using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonHolder : MonoBehaviour, IAudioPlayable
{
    public GameObject button;
    public event Action<ButtonHolderClickEventArgs> OnClick;

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
        if (gameObject == button)
        {
            OnClick?.Invoke(new ButtonHolderClickEventArgs() { buttonHolder = this, button = button });
            if (AudioSources.Count != 0)
                PlaySound(AudioSources[0]);
        }
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }
}

public class ButtonHolderClickEventArgs : EventArgs
{
    public ButtonHolder buttonHolder;
    public GameObject button;
}
