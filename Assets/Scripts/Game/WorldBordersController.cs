using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBordersController : Ston<WorldBordersController>
{
    public MeshRenderer leftBorder;
    public MeshRenderer rightBorder;
    public MeshRenderer downBorder;
    public PhysicMaterial tirePhysMaterial;
    private Transform _playerPos;
    private Rigidbody _playerRb;
    private float _bouncinessCoef;

    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _playerRb = _playerPos.GetComponent<Rigidbody>();
        _bouncinessCoef = Mathf.Sqrt(tirePhysMaterial.bounciness + 0.09f);
    }

    private void Update()
    {
        leftBorder.transform.position = new Vector3(leftBorder.transform.position.x,
                                          _playerPos.position.y,
                                          _playerPos.position.z);
        rightBorder.transform.position = new Vector3(rightBorder.transform.position.x,
                                          _playerPos.position.y,
                                          _playerPos.position.z);
        downBorder.transform.position = new Vector3(_playerPos.position.x,
                                          downBorder.transform.position.y,
                                          _playerPos.position.z);
    }

    private void FixedUpdate()
    {
        Vector3 collisionPoint = default;
        Vector3 normalVector = default;
        if (_playerPos.position.x < leftBorder.transform.position.x)
        {
            collisionPoint = leftBorder.transform.position;
            normalVector = Vector3.right;
            _playerPos.position = new Vector3(leftBorder.transform.position.x,
                                              _playerPos.position.y,
                                              _playerPos.position.z);
        }
        else if (_playerPos.position.x > rightBorder.transform.position.x)
        {
            collisionPoint = rightBorder.transform.position;
            normalVector = Vector3.left;
            _playerPos.position = new Vector3(rightBorder.transform.position.x,
                                              _playerPos.position.y,
                                              _playerPos.position.z);
        }

        if (collisionPoint != default)
        {
            Vector3 reflectForceVector = Vector3.Reflect(_playerRb.velocity, normalVector) * _bouncinessCoef;
            _playerRb.velocity = reflectForceVector;
        }
    }
}
