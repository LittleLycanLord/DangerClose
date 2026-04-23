using UnityEngine;
using System.Collections.Generic;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    [System.Serializable]
    public class FalloffSettings
    {
        [Header("Method")]
        public FalloffGenerator.FalloffGenerationMethod generationMethod = FalloffGenerator.FalloffGenerationMethod.ClassicCurve;

        [Header("Classic Curve")]
        public float classicCurveA = 3f;
        public float classicCurveB = 2.2f;

        [Header("SmoothStep")]
        [Range(0f, 1f)]
        public float smoothStepEdgeStart = 0.35f;
        [Range(0f, 1f)]
        public float smoothStepEdgeEnd = 0.85f;

        [Header("Power")]
        [Min(0.0001f)]
        public float powerExponent = 3f;

        [Header("Exponential")]
        [Min(0.0001f)]
        public float exponentialStrength = 4f;

        [Header("Circular")]
        [Range(0f, 1f)]
        public float circularRadius = 0.8f;
        [Min(0.0001f)]
        public float circularSoftness = 0.15f;
    }

    public static class FalloffGenerator
    {
        public enum FalloffGenerationMethod
        {
            ClassicCurve,
            SmoothStep,
            Power,
            Exponential,
            Circular
        }

        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        // Any references to any component/class should be placed here, serialized or not.

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        // Any non-component variables NOT SHOWN in the Inspector should be placed here.

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // Any non-component READ-ONLY variables SHOWN in the Inspector should be placed here.
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        // Any non-component READ-WRITE variables SHOWN in the Inspector should be placed here.
        // [Space(10)]
        // [Header("Fields")]
        public static FalloffGenerationMethod generationMethod = FalloffGenerationMethod.ClassicCurve;

        // ClassicCurve method fields
        public static float classicCurveA = 3f;
        public static float classicCurveB = 2.2f;

        // SmoothStep method fields
        public static float smoothStepEdgeStart = 0.35f;
        public static float smoothStepEdgeEnd = 0.85f;

        // Power method fields
        public static float powerExponent = 3f;

        // Exponential method fields
        public static float exponentialStrength = 4f;

        // Circular method fields
        public static float circularRadius = 0.8f;
        public static float circularSoftness = 0.15f;

        public static void ApplySettings(FalloffSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            generationMethod = settings.generationMethod;
            classicCurveA = settings.classicCurveA;
            classicCurveB = settings.classicCurveB;
            smoothStepEdgeStart = settings.smoothStepEdgeStart;
            smoothStepEdgeEnd = settings.smoothStepEdgeEnd;
            powerExponent = settings.powerExponent;
            exponentialStrength = settings.exponentialStrength;
            circularRadius = settings.circularRadius;
            circularSoftness = settings.circularSoftness;
        }

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        // Any Monobehaviour functions should be placed here.
        // void Awake() 
        // {

        // }

        // void Start() 
        // {

        // }

        // void Update() 
        // {

        // }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        // Any Non-Monobehaviour/custom functions should be placed here.
        // Note: Abstract/virtual functions or overrides have a separate section.
        public static float[,] GenerateFalloffMap(int size)
        {
            float[,] map = new float[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = i / (float)size * 2 - 1;
                    float y = j / (float)size * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    map[i, j] = Evaluate(value);
                }
            }

            return map;
        }

        private static float Evaluate(float value)
        {
            value = Mathf.Clamp01(value);

            switch (generationMethod)
            {
                case FalloffGenerationMethod.ClassicCurve:
                    {
                        float a = Mathf.Max(0.0001f, classicCurveA);
                        float b = Mathf.Max(0.0001f, classicCurveB);
                        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
                    }

                case FalloffGenerationMethod.SmoothStep:
                    {
                        float edgeStart = Mathf.Clamp01(smoothStepEdgeStart);
                        float edgeEnd = Mathf.Clamp01(smoothStepEdgeEnd);
                        if (edgeEnd < edgeStart)
                        {
                            float temp = edgeStart;
                            edgeStart = edgeEnd;
                            edgeEnd = temp;
                        }
                        return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(edgeStart, edgeEnd, value));
                    }

                case FalloffGenerationMethod.Power:
                    {
                        float exponent = Mathf.Max(0.0001f, powerExponent);
                        return Mathf.Pow(value, exponent);
                    }

                case FalloffGenerationMethod.Exponential:
                    {
                        float strength = Mathf.Max(0.0001f, exponentialStrength);
                        float normalized = (Mathf.Exp(value * strength) - 1f) / (Mathf.Exp(strength) - 1f);
                        return Mathf.Clamp01(normalized);
                    }

                case FalloffGenerationMethod.Circular:
                    {
                        float radius = Mathf.Clamp01(circularRadius);
                        float softness = Mathf.Max(0.0001f, circularSoftness);
                        return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(radius, radius + softness, value));
                    }

                default:
                    return value;
            }
        }
        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
        // Any Abstract/virtual functions or overrides should be placed here.

    }
}