using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    public static class MeshGenerator
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        // Any references to any component/class should be placed here, serialized or not.

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        // Any non-component variables NOT SHOWN in the Inspector should be placed here.
        public static readonly int[] LODIncrements = new[] {1, 2, 4, 6, 8, 10, 12};
        //! NOTE: mapChunkSize must be divisible by all values in LODIncrements to avoid errors. Formula: LCM + 1 (for the last vertex) = mapChunkSize. Example: LCM(1, 2, 4, 6, 8, 10, 12) = 120 + 1 = 121, so mapChunkSize could be 121, 241, etc.

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
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
        {
            AnimationCurve localHeightCurve = new AnimationCurve(heightCurve.keys);
            
            int meshSimplificationIncrement = LODIncrements[Mathf.Clamp(levelOfDetail, 0, LODIncrements.Length - 1)];

            int borderedSize = heightMap.GetLength(0);
            int meshSize = borderedSize - 2 * meshSimplificationIncrement;
            int meshSizeUnsimplified = borderedSize - 2;

            float topLeftX = (meshSizeUnsimplified - 1) / -2f;
            float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

            int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

            MeshData meshData = new MeshData(verticesPerLine);
            int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
            int meshVertexIndex = 0;
            int outOfMeshVertexIndex = -1;

            for (int y = 0; y < borderedSize; y+= meshSimplificationIncrement)
            {
                for (int x = 0; x < borderedSize; x+= meshSimplificationIncrement)
                {
                    bool isOutOfMeshVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;
                    if (isOutOfMeshVertex)
                    {
                        vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                        outOfMeshVertexIndex--;
                    }
                    else
                    {
                        vertexIndicesMap[x, y] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                }
            }

            for (int y = 0; y < borderedSize; y+= meshSimplificationIncrement)
            {
                for (int x = 0; x < borderedSize; x+= meshSimplificationIncrement)
                {
                    int vertexIndex = vertexIndicesMap[x, y];

                    Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
                    float height = localHeightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
                    Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

                    meshData.AddVertex(vertexPosition, percent, vertexIndex);

                    if(x < borderedSize - 1 && y < borderedSize - 1)
                    {
                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
                        int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                        int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                    
                    vertexIndex++;

                }
            }

            return meshData;
        }

        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
	    // Any Abstract/virtual functions or overrides should be placed here.
    }

    public class MeshData 
    {
        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;

        Vector3[] outOfMeshVertices;
        int[] outOfMeshTriangles;

        int triangleIndex;
        int outOfMeshTriangleIndex;

        public MeshData(int verticesPerLine)
        {
            vertices = new Vector3[verticesPerLine * verticesPerLine];
            uvs = new Vector2[verticesPerLine * verticesPerLine];
            triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
            outOfMeshVertices = new Vector3[verticesPerLine * 4 + 4];
            outOfMeshTriangles = new int[24 * verticesPerLine];
        }

        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
        {
            if(vertexIndex < 0)
            {
                outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
            }
            else
            {
                vertices[vertexIndex] = vertexPosition;
                uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if(a < 0 || b < 0 || c < 0)
            {
                outOfMeshTriangles[outOfMeshTriangleIndex] = a;
                outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
                outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
                outOfMeshTriangleIndex += 3;
            }
            else
            {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }
        }

        Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[vertices.Length];

            int triangleCount = triangles.Length / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = triangles[normalTriangleIndex];
                int vertexIndexB = triangles[normalTriangleIndex + 1];
                int vertexIndexC = triangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            int outOfMeshTriangleCount = outOfMeshTriangles.Length / 3;
            for (int i = 0; i < outOfMeshTriangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
                int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
                int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                if(vertexIndexA >= 0)
                    vertexNormals[vertexIndexA] += triangleNormal;
                if(vertexIndexB >= 0)
                    vertexNormals[vertexIndexB] += triangleNormal;
                if(vertexIndexC >= 0)
                    vertexNormals[vertexIndexC] += triangleNormal;
            }

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            Vector3 pointA = (indexA >= 0) ? vertices[indexA] : outOfMeshVertices[-indexA - 1];
            Vector3 pointB = (indexB >= 0) ? vertices[indexB] : outOfMeshVertices[-indexB - 1];
            Vector3 pointC = (indexC >= 0) ? vertices[indexC] : outOfMeshVertices[-indexC - 1];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = CalculateNormals();
            return mesh;
        }
    }
}