using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MenuPageHandler : MonoBehaviour, IMenuPage
{

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject _pageCamera;
    public PageTypes pageType;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Close()
    {
        animator.SetBool("IsPageOpen", false);
        _pageCamera.SetActive(false);
    }

    public void Open()
    {
        animator.SetBool("IsPageOpen", true);
        _pageCamera.SetActive(true);
    }
}
