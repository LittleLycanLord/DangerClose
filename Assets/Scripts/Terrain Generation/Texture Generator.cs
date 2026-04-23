using UnityEngine;
using System.Collections.Generic;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    public static class TextureGenerator
    {
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
        public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = heightMap[x, y];
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, value);
                }
            }

            return TextureFromColorMap(colorMap, width, height);
        }

        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
        // Any Abstract/virtual functions or overrides should be placed here.

    }
}