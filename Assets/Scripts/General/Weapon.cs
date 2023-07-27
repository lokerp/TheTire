using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public RuntimeAnimatorController animatorController;
    public Transform RHTargetInner;
    public Transform LHTargetInner;
    public Transform RLTargetInner;
    public Transform LLTargetInner;
    public ParticleSystem hitEffect;
    public List<AudioSource> sounds;
}
