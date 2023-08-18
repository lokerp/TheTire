using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : StonUndestroyable<ScenesManager>
{
    public Animator sceneTransition;
    private AsyncOperation asyncLoad;

    public static event Action OnSceneChanging;

    protected override void Awake()
    {
        base.Awake();
        sceneTransition.SetBool("IsOpening", true);
    }

    public void SwitchScene(string sceneName)
    {
        sceneTransition.SetBool("IsOpening", false);
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        OnSceneChanging?.Invoke();
        if (asyncLoad != null)
        {
            StartCoroutine(WaitForSceneLoad());
        }
        sceneTransition.SetBool("IsOpening", true);
    }

    IEnumerator WaitForSceneLoad()
    {
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
