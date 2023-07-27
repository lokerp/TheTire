using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptPage : Page
{
    [field: SerializeField]
    public ButtonHolder AcceptButton { get; private set; }
    [field: SerializeField]
    public ButtonHolder CancelButton { get; private set; }

    public event Action OnAccept;
    public event Action OnCancel;

    public bool closeOnCancel = true;
    

    protected override void OnEnable()
    {
        if (OpenButton != null)
            OpenButton.OnClick += Open;
        if (CloseButton != null)
            CloseButton.OnClick += Close;

        AcceptButton.OnClick += RaiseOnAcceptEvent;
        CancelButton.OnClick += RaiseOnCancelEvent;
        if (closeOnCancel)
            CancelButton.OnClick += Close;
    }

    protected override void OnDisable()
    {
        if (OpenButton != null)
            OpenButton.OnClick -= Open;
        if (CloseButton != null)
            CloseButton.OnClick -= Close;

        AcceptButton.OnClick -= RaiseOnAcceptEvent;
        CancelButton.OnClick -= RaiseOnCancelEvent;
        if (closeOnCancel)
            CancelButton.OnClick -= Close;
    }

    protected virtual void RaiseOnAcceptEvent()
    {
        OnAccept?.Invoke();
    }

    protected virtual void RaiseOnCancelEvent()
    {
        OnCancel?.Invoke();
    }
}
