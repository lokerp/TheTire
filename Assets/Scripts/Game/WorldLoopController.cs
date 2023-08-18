using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoopController : Ston<WorldLoopController>
{
    [field: SerializeField]
    public float WorldLoopSize { get; private set; }
    public int LoopsCount { get; private set; }
    public static event Action<Vector3> OnLoop = default;
    public static event Action<Vector3> AfterLoop = default;

    private Transform _playerPos;
    private Action<Vector3, float> OnLaunchHandler;

    protected override void Awake()
    {
        base.Awake();
        OnLaunchHandler = (_, _) => StartCoroutine(WorldLoop());
    }

    private void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void OnEnable()
    {
        LaunchController.OnLaunch += OnLaunchHandler;
    }

    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunchHandler;
    }

    public float GetRealPlayerZPosition()
    {
        return _playerPos.position.z + LoopsCount * WorldLoopSize;
    }

    IEnumerator WorldLoop()
    {
        LoopsCount = 0;
        while (true)
        {
            if (_playerPos.transform.position.z > WorldLoopSize
             || _playerPos.transform.position.z < -WorldLoopSize)
            {
                Vector3 translateVec = new()
                {
                    x = 0,
                    y = 0,
                    z = -_playerPos.transform.position.z
                };
                if (_playerPos.transform.position.z < 0)
                    LoopsCount--;
                else
                    LoopsCount++;
                OnLoop?.Invoke(translateVec);
                AfterLoop?.Invoke(translateVec);
            }
            yield return null;
        }
    }
}
