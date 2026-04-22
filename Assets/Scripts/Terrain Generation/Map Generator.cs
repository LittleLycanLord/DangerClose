using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(MapDisplay))]
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode { NoiseMap, ColorMap, FalloffMap, Mesh }
       

        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
	    // Any references to any component/class should be placed here, serialized or not.
        private MapDisplay mapDisplay;
        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
	    // Any non-component variables NOT SHOWN in the Inspector should be placed here.
        private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
        private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
        private float[,] falloffMap;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        [Tooltip("Determined by the LCM + 1 of the LOD increments in MeshGenerator.cs.")]
        public int mapChunkSize = 241;
	    // Any non-component READ-ONLY variables SHOWN in the Inspector should be placed here.

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [Header("Map Generation")]
        public DrawMode drawMode = DrawMode.ColorMap;
        public Noise.NormalizationMode normalizationMode = Noise.NormalizationMode.Local;
        public bool useFalloff = false;
        public FalloffSettings falloffSettings = new FalloffSettings();
        public List<TerrainType> regions = new List<TerrainType>();

        [Header("Noise Parameters")]
        [Range(1, 10)]
        [Tooltip("Higher values may cause performance issues.")]
        public int mapChunkSizeMultiple = 2;
        public int seed = 10;
        public float scale = 0.3f;
        [Range(1, 16)]
        public int octaves = 4;
        [Range(0, 1)]
        public float persistence = 0.5f;
        [Min(1f)]
        public float lacunarity = 2.0f;
        public Vector2 offset = Vector2.zero;

        [Header("Mesh Settings")]
        public float meshHeightMultiplier = 10;
        public AnimationCurve meshHeightCurve;
        [Range(0, 6)]
        public int meshLODPreview = 1;
	    // Any non-component READ-WRITE variables SHOWN in the Inspector should be placed here.

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
	    // Any Monobehaviour functions should be placed here.
        void Awake() { 
            mapChunkSize = CalculateLCM(MeshGenerator.LODIncrements) * mapChunkSizeMultiple + 1 - 2;
            FalloffGenerator.ApplySettings(falloffSettings);
            falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        }

        void Start() { 
            mapDisplay = GetComponent<MapDisplay>();
        }

        void Update() { 
            if(mapDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }

            if(meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
	    // Any Non-Monobehaviour/custom functions should be placed here.
	    // Note: Abstract/virtual functions or overrides have a separate section.
        public void DrawMapInEditor()
        {
            FalloffGenerator.ApplySettings(falloffSettings);
            MapData mapData = GenerateMapData(Vector2.zero);
            
            if (mapDisplay != null)
            {
                switch(drawMode) {
                    case DrawMode.NoiseMap:
                        mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                        break;
                    case DrawMode.ColorMap:
                        mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                        break;
                    case DrawMode.FalloffMap:
                        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
                        mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(falloffMap));
                        break;
                    case DrawMode.Mesh:
                        mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, meshLODPreview), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                        break;
                }
            }
        }

        public MapData GenerateMapData(Vector2 center)
        {
            FalloffGenerator.ApplySettings(falloffSettings);
            int calculatedMapChunkSize = CalculateLCM(MeshGenerator.LODIncrements) * mapChunkSizeMultiple + 1 - 2;
            if (mapChunkSize != calculatedMapChunkSize)
            {
                mapChunkSize = calculatedMapChunkSize;
                falloffMap = null;
            }

            int borderedMapSize = mapChunkSize + 2;

            float[,] noiseMap = Noise.GenerateNoiseMap(borderedMapSize, borderedMapSize, seed, scale, octaves, persistence, lacunarity, center + offset, normalizationMode);
            Color [] colorMap = new Color[mapChunkSize * mapChunkSize];

            if (regions != null && regions.Count > 1)
            {
                // regions.Sort((a, b) => b.height.CompareTo(a.height));
                // TerrainType highestRegion = regions[0];
                // highestRegion.height = 1f;
                // regions[0] = highestRegion;

                int lastRegionIndex = regions.Count - 1;
                TerrainType lowestRegion = regions[lastRegionIndex];
                lowestRegion.height = 0f;
                regions[lastRegionIndex] = lowestRegion;
            }

            for(int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    int borderedX = x + 1;
                    int borderedY = y + 1;

                    if(useFalloff)
                    {
                        if(falloffMap == null || falloffMap.GetLength(0) != mapChunkSize || falloffMap.GetLength(1) != mapChunkSize)
                        {
                            falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
                        }
                        noiseMap[borderedX, borderedY] = Mathf.Clamp01(noiseMap[borderedX, borderedY] - falloffMap[x, y]);
                    }

                    float currentHeight = noiseMap[borderedX, borderedY];

                    if (regions != null && regions.Count > 0)
                    {
                        int matchedRegionIndex = regions.Count - 1;
                        for (int i = 0; i < regions.Count; i++)
                        {
                            if (currentHeight >= regions[i].height)
                            {
                                matchedRegionIndex = i;
                                break;
                            }
                        }

                        colorMap[y * mapChunkSize + x] = regions[matchedRegionIndex].color;
                    }
                    else
                    {
                        colorMap[y * mapChunkSize + x] = Color.Lerp(Color.black, Color.white, currentHeight);
                    }
                }
            }
            return new MapData(noiseMap, colorMap);
        }

        public void RequestMapData(Vector2 center, Action<MapData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(center, callback);
            };

            new Thread(threadStart).Start();
        }

        private void MapDataThread(Vector2 center, Action<MapData> callback)
        {
            MapData mapData = GenerateMapData(center);
            lock (mapDataThreadInfoQueue)
            {
                mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
            }
        }

        public void RequestMeshData(MapData mapData, int levelOfDetail, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate 
            {
                MeshDataThread(mapData, levelOfDetail, callback);
            };

            new Thread(threadStart).Start();
        }

        private void MeshDataThread(MapData mapData, int levelOfDetail, Action<MeshData> callback)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
            lock (meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
            }
        }

        private int CalculateLCM(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
                return 1;
            
            int lcm = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                lcm = (lcm * numbers[i]) / GCD(lcm, numbers[i]);
            }
            return lcm;
        }

        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
	    // Any Abstract/virtual functions or overrides should be placed here.

        struct MapThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }

        public struct MeshDataThreadInfo
        {
            public readonly Action<MeshData> callback;
            public readonly MeshData parameter;

            public MeshDataThreadInfo(Action<MeshData> callback, MeshData parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
    
    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        [Range(0f, 1f)]
        public float height;
        public Color color;
    }
}
