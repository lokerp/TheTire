using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamsManager : Ston<CamsManager>
{
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera sideCamera;
    public float _noiseDuration;

    public float timeForCameraRotateInS;

    private CinemachineFramingTransposer _mainCameraTransposer;
    private GameObject _player;
    private Rigidbody _playerRb;
    private CinemachineBasicMultiChannelPerlin _noiseController;
    private float _noiseStrength;
    private float _noiseDefaultStrength = 1;
    private float _t;

    protected override void Awake()
    {
        base.Awake();
        _mainCameraTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _noiseController = mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        LaunchController.OnLaunch += OnLaunch;
        WorldLoopController.AfterLoop += OnAfterLoopHandler;
    }

    private void OnDisable()
    {
        LaunchController.OnLaunch -= OnLaunch;
        WorldLoopController.AfterLoop -= OnAfterLoopHandler;
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = _player.GetComponent<Rigidbody>();

        mainCamera.gameObject.SetActive(false);
        sideCamera.gameObject.SetActive(true);

        mainCamera.Follow = _player.transform;
        mainCamera.LookAt = _player.transform;
    }

    void OnAfterLoopHandler(Vector3 translateVec)
    {
        mainCamera.OnTargetObjectWarped(_player.transform, translateVec);
    }

    void OnLaunch(Vector3 force, float forceModifier)
    {
        mainCamera.gameObject.SetActive(true);
        sideCamera.gameObject.SetActive(false);

        _noiseStrength = forceModifier;
        _noiseController.m_FrequencyGain = _noiseStrength;

        StartCoroutine(DoCameraNoise());
    }

    IEnumerator DoCameraNoise()
    {
        while ((_t / _noiseDuration) < 1)
        {
            _t += Time.deltaTime;
            _noiseController.m_FrequencyGain = Mathf.Lerp(_noiseStrength,
                                                          _noiseDefaultStrength,
                                                          _t / _noiseDuration);
            yield return null;
        }
    }
}
