using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Unity.Mathematics;
using Unity.VisualScripting;

enum ScaleZone
{
    redZone,
    orangeZone,
    greenZone
}

public class LaunchController : MonoBehaviour, IAudioPlayable, IDataControllable, IAchievementsControllable
{
    public static LaunchController Instance { get; private set; }

    public RectTransform scale;
    public RectTransform arrow;
    public RectTransform redZone;
    public RectTransform orangeZone;

    public ParticleSystem launchEffect;
    public float smoothScaleArrowTime = 0.1f;
    public float maxScaleArrowVelocity = 3000f;
    public float maxAngle = 90;
    public float minAngle = 0;
    public int minForceModifier;
    public static event Action<Vector3, float> OnLaunch;
    public bool IsLaunched { get; private set; }
    public float randomXOffset;

    public GameObject launchAnimation;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; set; }
    public Action<AchievementProgress, byte> OnAchievementProgressChanged { get; set; }

    private Rigidbody _player;
    private int _powerLevel;
    private float _forceModifier;
    private PlayerInput _input;
    private float _amplitude;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _bottomPosition;
    private Vector3 _topPosition;
    private ScaleZone _zone;
    // 0 = down, 1 = up
    private byte _moveSide = 1;

    private int _redZoneThrowsCount;

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

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
        _input.Player.Launch.performed += (obj) => StartCoroutine(Launch());

        _amplitude = scale.sizeDelta.y / 2 - 20;
        _bottomPosition = new Vector3(arrow.localPosition.x, arrow.localPosition.y - _amplitude, arrow.localPosition.z);
        _topPosition = new Vector3(arrow.localPosition.x, arrow.localPosition.y + _amplitude, arrow.localPosition.z);

        arrow.localPosition = _bottomPosition;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        if (_player == null) Debug.LogError("Couldn't find object with tag 'player'");
    }

    private IEnumerator Launch()
    {
        if (!IsLaunched)
        {
            IsLaunched = true;

            Animator launchAnimator = launchAnimation.GetComponent<Animator>();
            AnimationEndHandler launchAnimationEndHandler = launchAnimation.GetComponent<AnimationEndHandler>();

            float angle = GetChoiceYPosition() / GetScaleHeight() * (maxAngle - minAngle);
            float bonusCoef = GetScaleBonus();

            if (_zone == ScaleZone.redZone)
            {
                _redZoneThrowsCount++;
                GetSniperAchievement(AchievementsManager.Instance.GetAchievementInfoById(11));
            }

            _powerLevel += 10;
            _forceModifier = (float)_powerLevel * (_powerLevel + 1) / 2 * 10;
            _player.constraints = RigidbodyConstraints.None;
            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 force = (_forceModifier * bonusCoef * new Vector3(0, sin, cos)) 
                            + new Vector3(UnityEngine.Random.Range(-randomXOffset, randomXOffset), 0, 0);

            launchAnimator.SetTrigger("Hit");
            while (launchAnimationEndHandler.IsAnimationEnded != true)
                yield return null;

            _player.AddForce(force, ForceMode.Impulse);
            PlaySound(AudioSources[0]);
            launchEffect.Play();
            OnLaunch.Invoke(force, _forceModifier);
        }
    }

    void Update()
    {
        if (IsLaunched == false)
        {
            if (_moveSide == 1)
            {
                arrow.localPosition = Vector3.SmoothDamp(arrow.localPosition, _topPosition, ref _velocity, smoothScaleArrowTime, maxScaleArrowVelocity);
                if (arrow.localPosition.y > _amplitude - 1)
                    _moveSide = 0;
            }
            else if (_moveSide == 0)
            {
                arrow.localPosition = Vector3.SmoothDamp(arrow.localPosition, _bottomPosition, ref _velocity, smoothScaleArrowTime, maxScaleArrowVelocity);
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
        return _zone switch
        {
            ScaleZone.redZone => 1.5f,
            ScaleZone.orangeZone => 1.2f,
            _ => 1f,
        };
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

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }

    public void SaveData(ref Database database)
    {
        database.redZoneThrowsCount = _redZoneThrowsCount;
    }

    public void LoadData(Database database)
    {
        _powerLevel = database.powerLevel;
        _redZoneThrowsCount = database.redZoneThrowsCount;
    }

    private void GetSniperAchievement(AchievementInfo achievement)
    {
        var progress = new AchievementProgress(_redZoneThrowsCount, achievement);
        OnAchievementProgressChanged.Invoke(progress, 11);
    }
}
