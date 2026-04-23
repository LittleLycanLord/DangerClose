using UnityEngine;
using System.Collections.Generic;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    public static class Noise
    {
        public enum NormalizationMode { Local, Global };

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
        // [Header("Displays")]
        // Any non-component READ-ONLY variables SHOWN in the Inspector should be placed here.

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        // [Space(10)]
        // [Header("Fields")]
        // Any non-component READ-WRITE variables SHOWN in the Inspector should be placed here.

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        // Any Monobehaviour functions should be placed here.
        // void Awake() { }

        // void Start() { }

        // void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        // Any Non-Monobehaviour/custom functions should be placed here.
        // Note: Abstract/virtual functions or overrides have a separate section.
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, NormalizationMode normalizationMode = NormalizationMode.Local)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random prng = new System.Random(seed);
            List<Vector2> octaveOffsets = new List<Vector2>();

            float maximumPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffsets.Add(new Vector2(offsetX, offsetY));

                maximumPossibleHeight += amplitude;
                amplitude *= persistence;
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            float maximumLocalNoiseHeight = float.MinValue;
            float minimumLocalNoiseHeight = float.MaxValue;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float localNoiseHeight = 0;

                    foreach (Vector2 octaveOffset in octaveOffsets)
                    {
                        float sampleX = (x - (mapWidth / 2f) + octaveOffset.x) / scale * frequency;
                        float sampleY = (y - (mapHeight / 2f) + octaveOffset.y) / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        localNoiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if (localNoiseHeight > maximumLocalNoiseHeight)
                    {
                        maximumLocalNoiseHeight = localNoiseHeight;
                    }
                    else if (localNoiseHeight < minimumLocalNoiseHeight)
                    {
                        minimumLocalNoiseHeight = localNoiseHeight;
                    }
                    noiseMap[x, y] = localNoiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    switch (normalizationMode)
                    {
                        case NormalizationMode.Local:
                            noiseMap[x, y] = Mathf.InverseLerp(minimumLocalNoiseHeight, maximumLocalNoiseHeight, noiseMap[x, y]);
                            break;
                        case NormalizationMode.Global:
                            float safeMaxPossibleHeight = Mathf.Max(maximumPossibleHeight, Mathf.Epsilon);
                            float normalizedHeight = (noiseMap[x, y] + safeMaxPossibleHeight) / (2f * safeMaxPossibleHeight);
                            noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0f, 1f);
                            break;
                    }
                }
            }

            return noiseMap;
        }
        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
        // Any Abstract/virtual functions or overrides should be placed here.

    }
}