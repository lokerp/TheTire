using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtension
{
    public static int Mod(int a, int b)
    {
        int c = a % b;
        return c < 0 ? c += b : c;
    }

    public static float Mod(float a, float b)
    {
        float c = a % b;
        return c < 0 ? c += b : c;
    }
}
