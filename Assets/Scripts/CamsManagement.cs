using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamsManagement : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject sideCamera;
    public PlayerController playerController;

    void Start()
    {
        mainCamera.SetActive(false);
        sideCamera.SetActive(true);
    }

    void Update()
    {
        if (playerController.isLaunched())
        {
            mainCamera.SetActive(true);
            sideCamera.SetActive(false);
        }

    }
}
