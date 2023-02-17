using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    [Serializable]
    public struct Info
    {
        public TranslatableText name;
        public TranslatableText description;
        public float mass;
        public float bounciness;
        public int cost;
    }

    [SerializeField] Info info;
}
