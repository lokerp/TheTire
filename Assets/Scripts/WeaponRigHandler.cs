using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigHandler : MonoBehaviour
{
    public TwoBoneIKConstraint RHRig;
    public TwoBoneIKConstraint LHRig;

    private Weapon _weapon;

    void Start()
    {
        _weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();

        RHRig.data.target = _weapon.RHTargetInner;
        LHRig.data.target = _weapon.LHTargetInner;
    }
}
