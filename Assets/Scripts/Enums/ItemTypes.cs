using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemTypes : IEquatable<ItemTypes>
{
    public enum Type
    {
        None,
        Tire,
        Weapon,
    }

    public struct Tire
    {
        public enum Type
        {
            None,
            DaddysSedan,
            Smarts,
        }
    }

    public struct Weapon
    {
        public enum Type
        {
            None,
            WoodenStick,
        }
    }

    public Type item;
    public Tire.Type tire;
    public Weapon.Type weapon;

    bool IEquatable<ItemTypes>.Equals(ItemTypes other)
    {
        if (item == other.item && tire == other.tire && weapon == other.weapon)
            return true;
        return false;
    }
}
