using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBordersController : Ston<WorldBordersController>
{
    public Transform leftBorder;
    public Transform rightBorder;
    public Transform downBorder;
    private Transform _playerPos;

    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        leftBorder.position = new Vector3(leftBorder.position.x,
                                          _playerPos.position.y,
                                          _playerPos.position.z);
        rightBorder.position = new Vector3(rightBorder.position.x,
                                          _playerPos.position.y,
                                          _playerPos.position.z);
        downBorder.position = new Vector3(_playerPos.position.x,
                                          downBorder.position.y,
                                          _playerPos.position.z);

        if (_playerPos.position.x < leftBorder.position.x - 5)
            _playerPos.position = new Vector3(leftBorder.position.x + 1,
                                              _playerPos.position.y,
                                              _playerPos.position.z);
        else if (_playerPos.position.x > rightBorder.position.x + 5)
            _playerPos.position = new Vector3(rightBorder.position.x - 1,
                                              _playerPos.position.y,
                                              _playerPos.position.z);
    }
}
