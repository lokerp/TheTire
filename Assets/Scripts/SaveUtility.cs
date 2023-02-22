using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveUtility
{
    public static T PathToPrefab<T>(string path)where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
}
