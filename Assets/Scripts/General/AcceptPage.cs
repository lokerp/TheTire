using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
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
        base.OnEnable();

        AcceptButton.OnClick += RaiseOnAcceptEvent;
        CancelButton.OnClick += RaiseOnCancelEvent;
        if (closeOnCancel)
            CancelButton.OnClick += closeBtnClickHandler;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        AcceptButton.OnClick -= RaiseOnAcceptEvent;
        CancelButton.OnClick -= RaiseOnCancelEvent;
        if (closeOnCancel)
            CancelButton.OnClick -= closeBtnClickHandler;
    }

    protected virtual void RaiseOnAcceptEvent(ButtonHolderClickEventArgs args = null)
    {
        OnAccept?.Invoke();
    }

    protected virtual void RaiseOnCancelEvent(ButtonHolderClickEventArgs args = null)
    {
        OnCancel?.Invoke();
    }
}
