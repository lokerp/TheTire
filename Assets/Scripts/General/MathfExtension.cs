using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    public enum FuncSpeed
    {
        Linear,
        Squared,
        SquareRoot,
        Smoothstep
    }

    public static float FuncSpeedApply(float value, FuncSpeed speed)
    {
        value = Mathf.Clamp01(value);
        switch (speed)
        {
            case FuncSpeed.Linear:
                break;
            case FuncSpeed.Squared:
                value *= value;
                break;
            case FuncSpeed.SquareRoot:
                value = Mathf.Sqrt(value);
                break;
            case FuncSpeed.Smoothstep:
                value = value * value * (3 - 2 * value);
                break;
        }
        return value;
    }

    public static IEnumerator Interpolation(float currentValue, float targetValue, float durationInS, FuncSpeed speed, Action<float> func)
    {
        float timeElapsedInS = 0;
        float progress;
        float difference = targetValue - currentValue;

        while (timeElapsedInS < durationInS)
        {
            timeElapsedInS += Time.deltaTime;
            progress = FuncSpeedApply(timeElapsedInS / durationInS, speed);
            func(currentValue + difference * progress);
            yield return null;
        }

        yield break;
    }
}
