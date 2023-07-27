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

    public void Start()
    {
        sceneTransition.SetTrigger("IsOpening");
    }

    public void SwitchScene(string sceneName)
    {
        sceneTransition.SetTrigger("IsClosing");
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        OnSceneChanging?.Invoke();
        if (asyncLoad != null)
        {
            StartCoroutine(WaitForSceneLoad());
        }
        sceneTransition.SetTrigger("IsOpening");
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
