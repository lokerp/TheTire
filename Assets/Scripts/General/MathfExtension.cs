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

    public static TranslatableText GetUnitsFromNumber(float num, bool translateToKm = false, bool isBigText = false, bool endsWithDot = false)
    {
        TranslatableText numPart = null;
        TranslatableText unitPart = null;
        if (!translateToKm || num < 10000)
        {
            numPart = (TranslatableText) ((long) num).ToString();
            if (isBigText)
                unitPart = new() { English = " M", Russian = " Ì"};
            else
                unitPart = new() { English = " m", Russian = " ל" };
        }
        else
        {
            numPart = (TranslatableText) Math.Round(num / 1000f, 1).ToString();
            if (isBigText)
                unitPart = new() { English = " KM", Russian = " ÊÌ" };
            else
                unitPart = new() { English = " km", Russian = " ךל" };
        }

        if (endsWithDot)
            unitPart.Concatenate(".");

        return numPart + unitPart;
    }

    public static TranslatableText GetUnitsFromNumber(long num, bool translateToKm = false, bool isBigText = false, bool endsWithDot = false)
    {
        TranslatableText numPart = null;
        TranslatableText unitPart = null;
        if (!translateToKm || num < 10000)
        {
            numPart = (TranslatableText)num.ToString();
            if (isBigText)
                unitPart = new() { English = " M", Russian = " Ì" };
            else
                unitPart = new() { English = " m", Russian = " ל" };
        }
        else
        {
            numPart = (TranslatableText)Math.Round(num / 1000f, 1).ToString();
            if (isBigText)
                unitPart = new() { English = " KM", Russian = " ÊÌ" };
            else
                unitPart = new() { English = " km", Russian = " ךל" };
        }

        if (endsWithDot)
            unitPart.Concatenate(".");

        return numPart + unitPart;
    }
}
