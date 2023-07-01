using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ScenesManager.OnSceneChanging += Close;
    }

    private void OnDisable()
    {
        ScenesManager.OnSceneChanging -= Close;
    }

    private void Start()
    {
        _animator.SetTrigger("IsOpening");
    }

    void Close()
    {
        _animator.SetTrigger("IsClosing");
    }
}
