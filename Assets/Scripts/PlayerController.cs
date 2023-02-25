using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public float forceModifier = 10;

    private float turnSpeed = 1;
    PlayerInput _input;
    Rigidbody _rigidbody;
    Vector3 _force;

    private void OnEnable()
    {
        _input.Enable();
        LaunchController.OnLaunch += Launch;
    }

    private void OnDisable()
    {
        _input.Disable();
        LaunchController.OnLaunch -= Launch;
    }

    private void Awake()
    {
        _input = new();
        _rigidbody = GetComponent<Rigidbody>();
        turnSpeed = 1 - Mathf.Clamp01((Mathf.Clamp(_rigidbody.mass, 5, 30) - 5) * 0.04f);
    }

    private void Launch(float angle, float bonusCoef)
    {
        _rigidbody.constraints = RigidbodyConstraints.None;
        float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
        _force = new Vector3(0, sin, cos) * forceModifier * bonusCoef;
        _rigidbody.AddForce(_force, ForceMode.Impulse);
    }

    private void Turn(float turnDirection)
    {
        _rigidbody.AddTorque(new Vector3(0, 0, turnDirection * turnSpeed * -1));
    }

    void Update()
    {
        if (LaunchController.Instance.IsLaunched)
        {
            float turnDirection = _input.Player.Turn.ReadValue<float>();
            Turn(turnDirection);
        }
    }
}
