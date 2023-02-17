using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public ForceChoice forceChoice;
    private bool _isLaunched;
    public float forceModifier = 10;
    public float maxAngle = 90;
    public float minAngle = 0;
    public float turnSpeed = 1;
    PlayerInput _input;
    Rigidbody _rigidbody;
    Vector3 _force;

    private void Awake()
    {
        _input = new();
        _input.Player.Launch.performed += context => 
        {
            if (!_isLaunched)
            {
                forceChoice._choiceMade = true;
                float angle = forceChoice.getChoiceYPosition() / forceChoice.getForceColumnHeight() 
                              * 100 * ((maxAngle - minAngle) / 100);

                Launch(angle);
                _isLaunched = true;
            }
        };

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Launch(float angle)
    {
        float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
        _force = new Vector3(0, sin, cos) * forceModifier;
        _rigidbody.AddForce(_force, ForceMode.Impulse);
    }

    private void Turn(float turnDirection)
    {
        _rigidbody.AddTorque(new Vector3(0, 0, turnDirection * turnSpeed * -1));
    }

    public bool isLaunched()
    {
        return _isLaunched;
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLaunched)
        {
            float turnDirection = _input.Player.Turn.ReadValue<float>();
            Turn(turnDirection);
        }

    }
}
