using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance { get; private set; }
    public Animator sceneTransition;
    private AsyncOperation asyncLoad;

    public static event Action OnSceneChanging;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SwitchScene(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        sceneTransition.SetTrigger("IsClosing");
        OnSceneChanging?.Invoke();
        if (asyncLoad != null)
        {
            StartCoroutine(WaitForSceneLoad());
        }
    }

    IEnumerator WaitForSceneLoad()
    {
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
        sceneTransition.SetTrigger("IsOpening");
    }
}
