using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour, IAchievementsControllable
{
    public static PlayerController Instance { get; private set; }

    public float waterDrag = 10;
    public float waterAngularDrag = 10;
    public float cloudDrag = 3;
    public float cloudAngularDrag = 3;

    private float _drag;
    private float _angularDrag;
    private float _turnSpeed = 1;
    PlayerInput _input;
    Rigidbody _rigidbody;

    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void OnDestroy()
    {
        OnAchievementProgressChanged = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _input = new();
        _rigidbody = GetComponent<Rigidbody>();
        _turnSpeed = 1 - Mathf.Clamp01((Mathf.Clamp(_rigidbody.mass, 5, 30) - 5) * 0.04f);
        _drag = _rigidbody.drag;
        _angularDrag = _rigidbody.angularDrag;
    }

    private void Turn(float turnDirection)
    {
        _rigidbody.AddTorque(new Vector3(0, 0, turnDirection * _turnSpeed * -1));
    }

    void Update()
    {
        if (LaunchController.Instance.IsLaunched)
        {
            float turnDirection = _input.Player.Turn.ReadValue<float>();
            Turn(turnDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            _rigidbody.drag = waterDrag;
            _rigidbody.angularDrag = waterAngularDrag;
            GetDiverAchievement();
        }

        else if (other.gameObject.CompareTag("Cloud"))
        {
            _rigidbody.drag = cloudDrag;
            _rigidbody.angularDrag = cloudAngularDrag;
            GetTodayIsCloudyAchievement();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _rigidbody.drag = _drag;
        _rigidbody.angularDrag = _angularDrag;
    }

    private void GetDiverAchievement()
    {
        var progress = new AchievementProgress(1, true);
        OnAchievementProgressChanged.Invoke(progress, 0);
    }

    private void GetTodayIsCloudyAchievement()
    {
        var progress = new AchievementProgress(1, true);
        OnAchievementProgressChanged.Invoke(progress, 1);
    }
}
