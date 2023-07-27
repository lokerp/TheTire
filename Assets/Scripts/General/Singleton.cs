using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ston<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = this.GetComponent<T>();
        else
            Destroy(gameObject);
    }
}

public abstract class StonUndestroyable<T> : Ston<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
