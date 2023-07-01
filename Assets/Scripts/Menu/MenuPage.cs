using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : Page
{
    public PageTypes pageType;

    [SerializeField] protected List<GraphicRaycaster> _canvasRaycasts;
    [SerializeField] protected GameObject _pageCamera;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void Close()
    {
        if (_animator != null)
            _animator.SetBool("IsPageOpen", false);
        if (_pageCamera != null)
            _pageCamera.SetActive(false);
        _canvasRaycasts.ForEach(r => r.enabled = false);
    }

    public override void Open()
    {
        if (_animator != null)
            _animator.SetBool("IsPageOpen", true);
        if (_pageCamera != null)
            _pageCamera.SetActive(true);
        _canvasRaycasts.ForEach(r => r.enabled = true);
    }
}
