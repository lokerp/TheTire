using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private SceneTransition instance;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += Initialize;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= Initialize;
    }

    private void Initialize(Scene current, Scene next)
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
