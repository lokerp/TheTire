using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : Ston<PlayerController>
{
    public Rigidbody Rigidbody { get; private set; }

    public float waterDrag = 10;
    public float waterAngularDrag = 10;
    public float cloudDrag = 3;
    public float cloudAngularDrag = 3;

    private float _drag;
    private float _angularDrag;
    private float _turnSpeed = 1;
    PlayerInput _input;

    public Action<GameObject, Vector3, Vector3> OnCollision { get; set; }

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
        OnCollision = null;
    }

    protected override void Awake()
    {
        base.Awake();

        _input = new();
        Rigidbody = GetComponent<Rigidbody>();
        _turnSpeed = 2;
        _drag = Rigidbody.drag;
        _angularDrag = Rigidbody.angularDrag;
    }

    private void Turn(float turnDirection)
    {
        Rigidbody.AddTorque(new Vector3(0, 0, turnDirection * _turnSpeed * -1));
    }

    void Update()
    {
        if (LaunchController.Instance.IsLaunched)
        {
            float turnDirection = Mathf.Clamp(_input.Player.Turn.ReadValue<float>(), -1, 1);
            Turn(turnDirection);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(collision.gameObject, collision.GetContact(0).point, collision.relativeVelocity);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            Rigidbody.drag = waterDrag;
            Rigidbody.angularDrag = waterAngularDrag;
        }

        else if (other.gameObject.CompareTag("Cloud"))
        {
            Rigidbody.drag = cloudDrag;
            Rigidbody.angularDrag = cloudAngularDrag;
        }

        OnCollision?.Invoke(other.gameObject, other.ClosestPoint(transform.position), Rigidbody.velocity);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody.drag = _drag;
        Rigidbody.angularDrag = _angularDrag;
    }
}
