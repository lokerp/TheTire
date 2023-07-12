using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigHandler : MonoBehaviour
{
    public Transform RHRigTarget;
    public Transform LHRigTarget;
    public Transform RLRigTarget;
    public Transform LLRigTarget;

    private Weapon _weapon;
    private Transform RHRigInnerTarget;
    private Transform LHRigInnerTarget;
    private Transform RLRigInnerTarget;
    private Transform LLRigInnerTarget;

    void Start()
    {
        _weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();

        RHRigInnerTarget = _weapon.RHTargetInner;
        LHRigInnerTarget = _weapon.LHTargetInner;
        RLRigInnerTarget = _weapon.RLTargetInner;
        LLRigInnerTarget = _weapon.LLTargetInner;
    }

    private void Update()
    {
        if (RHRigInnerTarget != null)
            RHRigTarget.SetPositionAndRotation(RHRigInnerTarget.position, RHRigInnerTarget.rotation);
        if (LHRigInnerTarget != null)
            LHRigTarget.SetPositionAndRotation(LHRigInnerTarget.position, LHRigInnerTarget.rotation);
        if (RLRigInnerTarget != null)
            RLRigTarget.SetPositionAndRotation(RLRigInnerTarget.position, RLRigInnerTarget.rotation);
        if (LLRigInnerTarget != null)
            LLRigTarget.SetPositionAndRotation(LLRigInnerTarget.position, LLRigInnerTarget.rotation);
    }
}
