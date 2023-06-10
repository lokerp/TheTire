using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public float timeOpenedInS;
    private Animator _animator;
    [SerializeField] private Image _icon;
    [SerializeField] private TextController _notificationTitle;
    [SerializeField] private TextController _info;

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
        StartCoroutine(Open(timeOpenedInS));
    }

    private IEnumerator Open(float timeOpenedInS)
    {
        _animator.SetBool("IsNotificationOpen", true);
        yield return new WaitForSecondsRealtime(timeOpenedInS);
        _animator.SetBool("IsNotificationOpen", false);
    }
}
