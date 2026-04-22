using UnityEngine;
using System.Collections.Generic;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(MapGenerator))]
    public class EndlessTerrainGenerator : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        // Any references to any component/class should be placed here, serialized or not.
        public Transform viewer;
        public Transform terrainChunkPool;
        private static MapGenerator mapGenerator;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
	    // Any non-component variables NOT SHOWN in the Inspector should be placed here.
        [HideInInspector]
        //: Note: This position is along the XZ plane.
        public static Vector2 viewerPosition;
        [HideInInspector]
        public static Vector2 oldViewerPosition;
        [HideInInspector]
        public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        [HideInInspector]
        static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // Any non-component READ-ONLY variables SHOWN in the Inspector should be placed here.
	    [Header("Displays")]
        public int chunksVisibleInViewDistance = 5;
        public static float maxViewDistance = 450;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
	    // Any non-component READ-WRITE variables SHOWN in the Inspector should be placed here.
        [Space(10)]
        [Header("Fields")]
        public float minimumMovementBeforeChunkUpdate = 25f;
        public Vector3 terrainScale = Vector3.one;

        [Range(0, 10)]
        public int maxChunkDistance = 3;
        // public const float maxViewDistance = 450;
        public Material terrainMeshMaterial;
        public List<LODInfo> levelsOfDetail = new List<LODInfo>();

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
	    // Any Monobehaviour functions should be placed here.
        void Awake() 
        {

        }

        void Start() 
        {
            mapGenerator = GetComponent<MapGenerator>();
            int chunkSize = mapGenerator.mapChunkSize - 1;
            chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);

            viewerPosition = new Vector2(viewer.position.x / terrainScale.x, viewer.position.z / terrainScale.z);
            maxViewDistance = maxChunkDistance * chunkSize;
            UpdateVisibleChunks(maxViewDistance);
        }

        void Update() 
        {
            int chunkSize = mapGenerator.mapChunkSize - 1;
            viewerPosition = new Vector2(viewer.position.x / terrainScale.x, viewer.position.z / terrainScale.z);
            maxViewDistance = maxChunkDistance * chunkSize;

            if ((viewerPosition - oldViewerPosition).sqrMagnitude >= minimumMovementBeforeChunkUpdate * minimumMovementBeforeChunkUpdate)
            {
                oldViewerPosition = viewerPosition;
                UpdateVisibleChunks(maxViewDistance);
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        // Any Non-Monobehaviour/custom functions should be placed here.
        // Note: Abstract/virtual functions or overrides have a separate section.
        void UpdateVisibleChunks(float viewDistance)
        {
            int chunkSize = mapGenerator.mapChunkSize - 1;

            for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) 
            {
                terrainChunksVisibleLastUpdate[i].SetVisible(false);
            }
            terrainChunksVisibleLastUpdate.Clear();
                
            int currentChunkXCoordinate = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkYCoordinate = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) 
            {
                for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
                {
                    Vector2 viewedChunkCoordinate = new Vector2(currentChunkXCoordinate + xOffset, currentChunkYCoordinate + yOffset);

                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoordinate)) 
                    {
                        terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk(viewDistance);
                        if (terrainChunkDictionary[viewedChunkCoordinate].IsVisible()) 
                        {
                            terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoordinate]);
                        }
                    } else {
                        terrainChunkDictionary.Add(viewedChunkCoordinate, 
                        new TerrainChunk(viewedChunkCoordinate, 
                                         chunkSize, 
                                         terrainChunkPool,
                                         terrainMeshMaterial, 
                                         levelsOfDetail,
                                         terrainScale
                        ));
                    }

                }
            }
        }

        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
	    // Any Abstract/virtual functions or overrides should be placed here.

        public class TerrainChunk 
        {
            //* ╔════════════╗
            //* ║ Components ║
            //* ╚════════════╝
            // Any references to any component/class should be placed here, serialized or not.
            MeshRenderer meshRenderer;
            MeshFilter meshFilter;
            MeshCollider meshCollider;

            //* ╔════════════╗
            //* ║ Attributes ║
            //* ╚════════════╝
            // Any non-component variables NOT SHOWN in the Inspector should be placed here.
            GameObject meshObject;
            Vector2 position;
            Bounds bounds;
            Vector3 terrainScale;

            List<LODInfo> levelsOfDetail;
            List<ChunkLODMesh> LODMeshes;
            MapData mapData;
            bool mapDataReceived;
            int previousLevelOfDetail = -1;

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
            public TerrainChunk(Vector2 coordinates, int size, Transform parent, Material material, List<LODInfo> levelsOfDetail, Vector3 terrainScale)
            {
                this.levelsOfDetail = levelsOfDetail;
                this.terrainScale = terrainScale;
                LODMeshes = new List<ChunkLODMesh>();
                foreach(LODInfo levelOfDetail in levelsOfDetail) 
                {
                    LODMeshes.Add(new ChunkLODMesh(levelOfDetail.levelOfDetail, () => UpdateTerrainChunk(maxViewDistance)));
                }

                position = coordinates * size;
                bounds = new Bounds(position, Vector2.one * size);
                Vector3 chunkPosition = new Vector3(position.x, 0, position.y);

                meshObject = new GameObject("Terrain Chunk");
                meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshCollider = meshObject.AddComponent<MeshCollider>();
                meshRenderer.material = material;

                meshObject.transform.position = Vector3.Scale(chunkPosition, terrainScale);
                meshObject.transform.parent = parent;
                meshObject.transform.localScale = terrainScale;

                SetVisible(false);

                mapGenerator.RequestMapData(position, OnMapDataReceived);
            }

            void OnMapDataReceived(MapData mapData) 
            {
                this.mapData = mapData;
                mapDataReceived = true;

                Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, mapGenerator.mapChunkSize, mapGenerator.mapChunkSize);
                meshRenderer.material.mainTexture = texture;

                UpdateTerrainChunk(maxViewDistance);
            }

            public void UpdateTerrainChunk(float viewDistance) 
            {
                if(!mapDataReceived) return;

                float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistanceFromNearestEdge <= viewDistance;

                if(visible) 
                {
                    int currentLevelOfDetail = 0;
                    for (int i = 0; i < levelsOfDetail.Count - 1; i++) 
                    {
                        if (viewerDistanceFromNearestEdge > levelsOfDetail[i].chunkDistanceThreshold) 
                        {
                            currentLevelOfDetail = i + 1;
                        } 
                        else 
                        {
                            break;
                        }
                    }

                    if(currentLevelOfDetail != previousLevelOfDetail) 
                    {
                        if (LODMeshes[currentLevelOfDetail].hasMesh) 
                        {
                            previousLevelOfDetail = currentLevelOfDetail;
                            meshFilter.mesh = LODMeshes[currentLevelOfDetail].mesh;
                            meshCollider.sharedMesh = LODMeshes[currentLevelOfDetail].mesh;
                        } 
                        else if (!LODMeshes[currentLevelOfDetail].hasRequestedMesh) 
                        {
                            LODMeshes[currentLevelOfDetail].RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(visible);
            }

            public void SetVisible(bool visible) 
            {
                meshObject.SetActive(visible);
            }

            public bool IsVisible() 
            {
                return meshObject.activeSelf;
            }

            //* ╔════════════════════════════════╗
            //* ║ Virtual / Overridden Functions ║
            //* ╚════════════════════════════════╝
            // Any Abstract/virtual functions or overrides should be placed here.

        }

        class ChunkLODMesh 
        {
            public Mesh mesh;
            public bool hasRequestedMesh;
            public bool hasMesh;

            private MapData mapData;
            private int levelOfDetail;
            private System.Action callback;

            public ChunkLODMesh(int levelOfDetail, System.Action callback) 
            {
                this.levelOfDetail = levelOfDetail;
                this.callback = callback;
            }

            void OnMeshDataReceived(MeshData meshData) 
            {
                mesh = meshData.CreateMesh();
                hasMesh = true;

                callback();
            }

            public void RequestMesh(MapData mapData) 
            {
                this.mapData = mapData;
                hasRequestedMesh = true;
                mapGenerator.RequestMeshData(mapData, levelOfDetail, OnMeshDataReceived);
            }
        }

        [System.Serializable]
        public struct LODInfo 
        {
            public int levelOfDetail;
            public float chunkDistanceThreshold;

            public LODInfo(int levelOfDetail, float chunkDistanceThreshold) 
            {
                this.levelOfDetail = levelOfDetail;
                this.chunkDistanceThreshold = chunkDistanceThreshold;
            }
        }
    }    
}