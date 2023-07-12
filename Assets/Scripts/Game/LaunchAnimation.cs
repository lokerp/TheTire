using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAnimation : MonoBehaviour
{
    private Weapon _weapon;

    public void Start()
    {
        _weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();
    }

    public void PlaySound(int index)
    {
        _weapon.sounds[index].Play();
    }

    public void PlayHitEffect()
    {
        _weapon.hitEffect.Play();
    }
}
