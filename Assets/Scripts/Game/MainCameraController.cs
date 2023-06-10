using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [SerializeField]GameObject _player;
    Vector3 playerPosition;
    [SerializeField]Vector3 _offset = new(0, 1, -3);
    [Range(1, 10)][SerializeField]float _smoothSpeed;

    void LateUpdate()
    {
        playerPosition = _player.transform.position + _offset;
        transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * _smoothSpeed);
    }
}
