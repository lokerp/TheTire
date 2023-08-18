using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    public Material skyMaterial;
    public Material starsMaterial;
    public Transform stars;
    public Transform sun;

    private Vector3 _defaultSkyScale;
    private Vector3 _defaultSkyPosition;
    private Vector3 _startPlayerPos;
    private Transform _currentPlayerPos;
    private Rigidbody _playerRb;
    private MeshRenderer _skyMesh;
    private float _nonScaleMeshSize;

    [Range(0f, 1f)] public float startTimeOfDay = 0.3f;
    public float timeOfDayCycleInM = 2400f;

    private void Awake()
    {
        _skyMesh = GetComponent<MeshRenderer>();
        _defaultSkyScale = transform.localScale;
        _defaultSkyPosition = transform.position;
        skyMaterial.mainTextureOffset = new Vector2(startTimeOfDay, 0);
    }

    void Start()
    {
        _currentPlayerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _startPlayerPos = _currentPlayerPos.position;
        _playerRb = _currentPlayerPos.GetComponent<Rigidbody>();
        _nonScaleMeshSize = _skyMesh.bounds.size.y / _defaultSkyScale.y;
    }


    void LateUpdate()
    {
        UpscaleSky();
        ChangeTimeOfDay();
    }

    void UpscaleSky()
    {
        float newYPosition = _defaultSkyPosition.y;
        if (_currentPlayerPos.position.y > 1000)
            newYPosition = Mathf.Lerp(transform.position.y, 0.02f * _currentPlayerPos.position.y, Time.deltaTime);
        else if (!Mathf.Approximately(transform.position.y, _defaultSkyPosition.y))
            newYPosition = Mathf.Lerp(transform.position.y, _defaultSkyPosition.y, Time.deltaTime * 5);

        transform.position = new Vector3(transform.position.x, newYPosition, _currentPlayerPos.position.z);

        if (_playerRb.velocity.y > 0 && _currentPlayerPos.position.y / _skyMesh.bounds.size.y > 0.8f
            || transform.localScale.y > _defaultSkyScale.y)
        {
            float newYScale = transform.localScale.y + (_playerRb.velocity.y / _nonScaleMeshSize);
            Vector3 sizeAdjVector = new(transform.localScale.x,
                                        newYScale,
                                        transform.localScale.z);
            transform.localScale = Vector3.Lerp(transform.localScale, sizeAdjVector, Time.deltaTime);
        }

        else if (!Mathf.Approximately(transform.localScale.y, _defaultSkyScale.y))
            transform.localScale = Vector3.Lerp(transform.localScale, _defaultSkyScale, Time.deltaTime * 5);
    }

    void ChangeTimeOfDay()
    {
        float playerDistance = WorldLoopController.Instance.GetRealPlayerZPosition() - _startPlayerPos.z;
        float distanceToDayCycleRatio;

        if (playerDistance >= 0)
            distanceToDayCycleRatio = playerDistance / timeOfDayCycleInM;
        else
            distanceToDayCycleRatio = MathfExtension.Mod(playerDistance, timeOfDayCycleInM) / timeOfDayCycleInM;

        Vector2 newOffset = new((startTimeOfDay + distanceToDayCycleRatio) % 1, 0);
        skyMaterial.mainTextureOffset = newOffset;

        float starsOpacity = 0;
        if ((0f <= newOffset.x && newOffset.x <= 0.2f) || (0.8f <= newOffset.x && newOffset.x <= 1f))
            starsOpacity = 1;

        starsMaterial.color = new Color(1, 1, 1, Mathf.Lerp(starsMaterial.color.a, starsOpacity, 5 * Time.deltaTime));
        stars.Rotate(0, .1f * _playerRb.velocity.z * Time.deltaTime, 0, Space.Self);

        sun.transform.localRotation = Quaternion.Euler(-360 * Mathf.Pow(newOffset.x - 0.5f, 5) * Mathf.Pow(2, 4), 180, 18);
    }
}
