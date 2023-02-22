using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TypesUtility
{
    public static class Item
    {
        public enum Type
        {
            None,
            Tire,
            Weapon,
        }

        public static class Tire
        {
            public enum Type
            {
                None,
                DaddysSedan,
                Smarts,
            }
        }

        public static class Weapon
        {
            public enum Type
            {
                None,
                zalupa = 1,
            }
       }
    }
}
