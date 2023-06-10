using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuPageHandler : MonoBehaviour, IMenuPage
{

    [SerializeField] private List<GraphicRaycaster> canvasRaycasts;
    [SerializeField] private GameObject _pageCamera;
    public PageTypes pageType;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Close()
    {
        if (animator != null)
            animator.SetBool("IsPageOpen", false);
        if (_pageCamera != null)
            _pageCamera.SetActive(false);
        canvasRaycasts.ForEach(r => r.enabled = false);
    }

    public void Open()
    {
        if (animator != null)
            animator.SetBool("IsPageOpen", true);
        if (_pageCamera != null)
            _pageCamera.SetActive(true);
        canvasRaycasts.ForEach(r => r.enabled = true);
    }
}
