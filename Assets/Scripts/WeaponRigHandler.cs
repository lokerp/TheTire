using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigHandler : MonoBehaviour
{
    public Transform RHRigTarget;
    public Transform LHRigTarget;

    private Weapon _weapon;
    private Transform RHRigInnerTarget;
    private Transform LHRigInnerTarget;

    void Start()
    {
        _weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();

        RHRigInnerTarget = _weapon.RHTargetInner;
        LHRigInnerTarget = _weapon.LHTargetInner;
    }

    private void Update()
    {
        RHRigTarget.SetPositionAndRotation(RHRigInnerTarget.position, RHRigInnerTarget.rotation);
        LHRigTarget.SetPositionAndRotation(LHRigInnerTarget.position, LHRigInnerTarget.rotation);
    }
}
