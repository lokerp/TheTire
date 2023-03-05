using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamsManager : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera sideCamera;
    public float _noiseDuration;

    private GameObject _player;
    private CinemachineBasicMultiChannelPerlin _noiseController;
    private float _noiseStrength;
    private float _noiseDefaultStrength = 1;
    private float _t;

    private void OnEnable()
    {
        LaunchController.OnLaunch += OnLaunch;
    }

    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunch;
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _noiseController = mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        mainCamera.gameObject.SetActive(false);
        sideCamera.gameObject.SetActive(true);

        mainCamera.Follow = _player.transform;
        mainCamera.LookAt = _player.transform;
    }

    void OnLaunch(float forceModifier)
    {
        SwitchCamera(forceModifier);
        StartCoroutine(DoCameraNoise());
    }

    private void SwitchCamera(float forceModifier)
    {
        mainCamera.gameObject.SetActive(true);
        sideCamera.gameObject.SetActive(false);

        _noiseStrength = forceModifier;
        _noiseController.m_FrequencyGain = _noiseStrength;
    }

    IEnumerator DoCameraNoise()
    {
        while ((_t / _noiseDuration) < 1)
        {
            _t += Time.deltaTime;
            _noiseController.m_FrequencyGain = Mathf.Lerp(_noiseStrength, _noiseDefaultStrength, _t / _noiseDuration);
            yield return null;
        }
    }


}
