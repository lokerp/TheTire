using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Settings
{
    public Languages currentLanguage;
    [Range(0, 10)] public int currentMusicVolume;
    [Range(0, 10)] public int currentSoundsVolume;
}
