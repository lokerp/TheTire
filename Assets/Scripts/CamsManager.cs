using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamsManager : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera sideCamera;
    public GameObject player;
    public float _noiseDuration;

    private CinemachineBasicMultiChannelPerlin _noiseController;
    private float _noiseStrength;
    private float _noiseDefaultStrength = 1;
    private float _t;

    private void OnEnable()
    {
        LaunchController.OnLaunch += SwitchCamera;
    }

    private void OnDisable()
    {
        LaunchController.OnLaunch -= SwitchCamera;
    }

    private void SwitchCamera(float angle, float bonusCoef)
    {
        mainCamera.gameObject.SetActive(true);
        sideCamera.gameObject.SetActive(false);

        float force = player.GetComponent<PlayerController>().forceModifier;

        _noiseStrength = force * bonusCoef;
        _noiseController.m_FrequencyGain = _noiseStrength;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _noiseController = mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        mainCamera.gameObject.SetActive(false);
        sideCamera.gameObject.SetActive(true);

        mainCamera.Follow = player.transform;
        mainCamera.LookAt = player.transform;
    }

    private void Update()
    {
        if (LaunchController.Instance.IsLaunched && (_t / _noiseDuration) < 1)
        {
            _t += Time.deltaTime;
            _noiseController.m_FrequencyGain = Mathf.Lerp(_noiseStrength, _noiseDefaultStrength, _t / _noiseDuration);
        }
    }


}
