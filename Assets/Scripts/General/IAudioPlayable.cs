using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioPlayable
{
    public List<AudioSource> AudioSources { get; }
    public void PlaySound(AudioSource source);
}
