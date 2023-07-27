using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct Settings : IEquatable<Settings>
{
    public Languages language;
    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float soundsVolume;

    public bool Equals(Settings other)
    {
        return this.language == other.language
            && Mathf.Approximately(this.musicVolume, other.musicVolume)
            && Mathf.Approximately(this.soundsVolume, other.soundsVolume);
    }

    public override bool Equals(object obj)
    {
        return obj is Settings other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(language, musicVolume, soundsVolume);
    }

    public static bool operator ==(Settings s1, Settings s2)
    {
        return s1.Equals(s2);
    }

    public static bool operator !=(Settings s1, Settings s2)
    {
        return !(s1 == s2);
    }
}
