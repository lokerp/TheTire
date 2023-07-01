using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    [field: SerializeField]
    public Canvas Canvas { get; private set; }

    [SerializeField] protected Animator _animator;

    [field: SerializeField]
    public bool HasOpeningAnimation { get; protected set; }
    [field: SerializeField]
    public bool HasClosingAnimation { get; protected set; }

    [field: SerializeField]
    public ButtonHolder OpenButton { get; protected set; }
    [field: SerializeField]
    public ButtonHolder CloseButton { get; protected set; }

    public event Action OnOpen;
    public event Action OnClose;

    protected virtual void OnEnable()
    {
        if (OpenButton != null)
            OpenButton.OnClick += Open;
        if (CloseButton != null)
            CloseButton.OnClick += Close;
    }

    protected virtual void OnDisable()
    {
        if (OpenButton != null)
            OpenButton.OnClick -= Open;
        if (CloseButton != null)
            CloseButton.OnClick -= Close;
    }

    private void Awake()
    {
        if (_animator == null && (HasOpeningAnimation || HasClosingAnimation))
            throw new System.Exception("Page Component doesn't have an animator, but has animations");
    }

    public virtual void Open()
    {
        if (HasOpeningAnimation)
            _animator.SetBool("IsOpen", true);
        else
            Canvas.enabled = true;
        OnOpen?.Invoke();
    }

    public virtual void Close()
    {
        if (HasClosingAnimation)
            _animator.SetBool("IsOpen", false);
        else
            Canvas.enabled = false;
        OnClose?.Invoke();
    }

    public bool IsOpened()
    {
        return Canvas.enabled;
    }

}
