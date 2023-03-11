using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public float waterDrag = 10;
    public float waterAngularDrag = 10;
    public float cloudDrag = 5;
    public float cloudAngularDrag = 5;

    private float _drag;
    private float _angularDrag;
    private float _turnSpeed = 1;
    PlayerInput _input;
    Rigidbody _rigidbody;

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Awake()
    {
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
        }

        else if (other.gameObject.CompareTag("Cloud"))
        {
            _rigidbody.drag = cloudDrag;
            _rigidbody.angularDrag = cloudAngularDrag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _rigidbody.drag = _drag;
        _rigidbody.angularDrag = _angularDrag;
    }
}
