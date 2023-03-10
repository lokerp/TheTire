using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    public Material skyMaterial;

    private Vector3 _defaultSkyScale;
    private Vector3 _startPlayerPos;
    private Transform _playerPos;
    private Rigidbody _playerRb;
    private MeshRenderer _skyMesh;
    private float _nonScaleMeshSize;

    private void Awake()
    {
        _skyMesh = GetComponent<MeshRenderer>();
        _defaultSkyScale = transform.localScale;
    }

    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _startPlayerPos = _playerPos.position;
        _playerRb = _playerPos.GetComponent<Rigidbody>();
        _nonScaleMeshSize = _skyMesh.bounds.size.y / _defaultSkyScale.y;
    }


    void LateUpdate()
    {
        UpscaleSky();

    }

    void UpscaleSky()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _playerPos.position.z);

        float newYCoordinates = transform.localScale.y + (_playerRb.velocity.y / _nonScaleMeshSize);

        Vector3 sizeAdjVector = new(transform.localScale.x,
                                    newYCoordinates,
                                    transform.localScale.z);

        if (_playerRb.velocity.y > 0 && _playerPos.position.y / _skyMesh.bounds.size.y > 0.8f
            || transform.localScale.y > _defaultSkyScale.y)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, sizeAdjVector, Time.deltaTime);
        }

        else if (!Mathf.Approximately(transform.localScale.y, _defaultSkyScale.y))
            transform.localScale = Vector3.Lerp(transform.localScale, _defaultSkyScale, 5);
    }
}
