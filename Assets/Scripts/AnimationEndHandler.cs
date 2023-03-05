using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEndHandler : MonoBehaviour
{
    public bool IsAnimationEnded { get; private set; }

    private void SetAnimationEnd()
    {
        IsAnimationEnded = true;
    }
}  
