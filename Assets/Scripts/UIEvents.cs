using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UIEvents : MonoBehaviour
{
    public static UIEvents Instance { get; private set; }

    private InputSystemUIInputModule UIInputSystem;
    public string layerName = "UI";
    public static event Action<GameObject> OnUIClick;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);

        UIInputSystem = GetComponent<InputSystemUIInputModule>();
    }

    void Update()
    {
        if (UIInputSystem.leftClick.action.WasReleasedThisFrame())
        {
            Vector2 clickPosition = UIInputSystem.point.action.ReadValue<Vector2>();
            CheckForOverlayUI(clickPosition);
        }
    }

    void CheckForOverlayUI(Vector2 clickPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = clickPos;
        List<RaycastResult> resultsData = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, resultsData);

        foreach (var result in resultsData)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                OnUIClick.Invoke(result.gameObject);
            }
        }
    }
}
