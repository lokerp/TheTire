using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderExtension : MonoBehaviour, IAudioPlayable
{
    public Slider slider;
    public TextMeshProUGUI textHolder;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChange);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChange);
    }

    private void OnSliderValueChange(float value)
    {
        textHolder.text = value.ToString() + "/" + slider.maxValue.ToString();
        PlaySound(AudioSources[0]);
    }

    private void Start()
    {
        OnSliderValueChange(slider.value);
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }
}
