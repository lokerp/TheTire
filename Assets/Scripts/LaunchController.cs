using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

enum ScaleZone
{
    redZone,
    orangeZone,
    greenZone
}

public class LaunchController : MonoBehaviour
{
    public static LaunchController Instance { get; private set; }

    public RectTransform scale;
    public RectTransform arrow;
    public RectTransform redZone;
    public RectTransform orangeZone;

    public ParticleSystem launchEffect;
    public float smoothTime = 2;
    public float maxVelocity = 5f;
    public float maxAngle = 90;
    public float minAngle = 0;
    public static event Action<float> OnLaunch;
    public bool IsLaunched { get; private set; }

    private Rigidbody _player;
    private float _forceModifier = 100;
    private PlayerInput _input;
    private float _amplitude;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _bottomPosition;
    private Vector3 _topPosition;
    private ScaleZone _zone;
    // 0 = down, 1 = up
    private byte _moveSide = 1;

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _input = new();
        _input.Player.Launch.performed += Launch;

        _amplitude = scale.sizeDelta.y / 2 - 20;
        _bottomPosition = new Vector3(arrow.localPosition.x, arrow.localPosition.y - _amplitude, arrow.localPosition.z);
        _topPosition = new Vector3(arrow.localPosition.x, arrow.localPosition.y + _amplitude, arrow.localPosition.z);

        arrow.localPosition = _bottomPosition;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

    }

    private void Launch(InputAction.CallbackContext obj)
    {
        if (!IsLaunched)
        {
            float angle = GetChoiceYPosition() / GetScaleHeight() * (maxAngle - minAngle);
            float bonusCoef = GetScaleBonus();
            _forceModifier *= bonusCoef;

            _player.constraints = RigidbodyConstraints.None;
            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 force = new Vector3(0, sin, cos) * _forceModifier;
            _player.AddForce(force, ForceMode.Impulse);

            launchEffect.Play();

            OnLaunch.Invoke(_forceModifier);
            IsLaunched = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLaunched == false)
        {
            if (_moveSide == 1)
            {
                arrow.localPosition = Vector3.SmoothDamp(arrow.localPosition, _topPosition, ref _velocity, smoothTime, maxVelocity);
                if (arrow.localPosition.y > _amplitude - 1)
                    _moveSide = 0;
            }
            else if (_moveSide == 0)
            {
                arrow.localPosition = Vector3.SmoothDamp(arrow.localPosition, _bottomPosition, ref _velocity, smoothTime, maxVelocity);
                if (arrow.localPosition.y < -_amplitude + 1)
                    _moveSide = 1;
            }

            CheckZone();
        }
    }
    
    float GetChoiceYPosition()
    {
        return GetScaleHeight() / 2 + arrow.localPosition.y;
    }

    float GetScaleHeight()
    {
        return scale.sizeDelta.y;
    }

    float GetScaleBonus()
    {
        switch (_zone)
        {
            case ScaleZone.redZone:
                return 1.5f;
            case ScaleZone.orangeZone:
                return 1.2f;
            default:
                return 1f;
        }
    }

    void CheckZone()
    {
        if (arrow.localPosition.y <= redZone.localPosition.y + redZone.sizeDelta.y / 2
            && arrow.localPosition.y >= redZone.localPosition.y - redZone.sizeDelta.y / 2)
        {
            _zone = ScaleZone.redZone;
            arrow.GetComponent<Image>().color = Color.red;
        }

        else if (arrow.localPosition.y <= orangeZone.localPosition.y + orangeZone.sizeDelta.y / 2
            && arrow.localPosition.y >= orangeZone.localPosition.y - orangeZone.sizeDelta.y / 2)
        {
            _zone = ScaleZone.orangeZone;
            arrow.GetComponent<Image>().color = Color.white;
        }

        else
        {
            _zone = ScaleZone.greenZone;
            arrow.GetComponent<Image>().color = Color.white;
        }
    }
}
