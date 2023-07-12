using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AnimatorController animatorController;
    public Transform RHTargetInner;
    public Transform LHTargetInner;
    public Transform RLTargetInner;
    public Transform LLTargetInner;
    public ParticleSystem hitEffect;
    public List<AudioSource> sounds;
}
