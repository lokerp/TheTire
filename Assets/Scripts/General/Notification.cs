using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour, IAudioPlayable
{
    public float timeOpenedInS;
    private Animator _animator;
    [SerializeField] private Image _icon;
    [SerializeField] private TextController _notificationTitle;
    [SerializeField] private TextController _info;
    [SerializeField] private TextMeshProUGUI _rewardText;

    [field: SerializeField]
    public List<AudioSource> AudioSources { get; private set; }

    private void OnEnable()
    {
        AchievementsManager.OnAchievementEarned += OpenWithAchievement;
    }

    private void OnDisable()
    {
        AchievementsManager.OnAchievementEarned -= OpenWithAchievement;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenWithAchievement(AchievementInfo achievement)
    {
        _icon.sprite = Resources.Load<Sprite>(achievement.imagePath);
        _info.text = achievement.title;
        _info.RefreshText();
        _rewardText.text = "+ " + achievement.moneyPrize.ToString();
        StartCoroutine(Open(timeOpenedInS));
    }

    private IEnumerator Open(float timeOpenedInS)
    {
        PlaySound(AudioSources[0]);
        _animator.SetBool("IsNotificationOpen", true);
        yield return new WaitForSecondsRealtime(timeOpenedInS);
        _animator.SetBool("IsNotificationOpen", false);
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }
}
