using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtension
{
    public static void PlayAtPosition(this AudioSource source, Vector3 position)
    {
        AudioSource instantiated = Object.Instantiate(source, position, source.gameObject.transform.rotation);
        instantiated.Play();
        Object.Destroy(instantiated.gameObject, source.clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

    public static IEnumerator FadeStop(this AudioSource source, float fadingTimeInS, MathfExtension.FuncSpeed fadingSpeed)
    {
        float defaultVolume = source.volume;
        IEnumerator func = MathfExtension.Interpolation(defaultVolume,
                                                        0,
                                                        fadingTimeInS,
                                                        fadingSpeed,
                                                        (value) => source.volume = value);
        yield return func;
        source.Stop();
        source.volume = defaultVolume;
        yield break;
    }
}
