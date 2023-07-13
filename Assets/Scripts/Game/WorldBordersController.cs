using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBordersController : MonoBehaviour
{
    public List<Transform> sideBorders;
    public Transform upBorder;
    public Transform downBorder;
    private Transform _playerPos;

    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        foreach (var border in sideBorders)
            border.position = new Vector3(border.position.x,
                                          _playerPos.position.y,
                                          _playerPos.position.z);

        downBorder.position = new Vector3(_playerPos.position.x,
                                          downBorder.position.y,
                                          _playerPos.position.z);
        upBorder.position = new Vector3(_playerPos.position.x,
                                        upBorder.position.y,
                                        _playerPos.position.z);
    }
}
