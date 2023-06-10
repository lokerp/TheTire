using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AnimationEndHandler))]
public class Weapon : MonoBehaviour
{
    [SerializeField] int _power;
    public int GetPower() => _power;

    public Transform RHTargetInner;
    public Transform LHTargetInner;
}
