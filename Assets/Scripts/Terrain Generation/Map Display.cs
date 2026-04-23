using UnityEngine;
using System.Collections.Generic;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    public class MapDisplay : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        // Any references to any component/class should be placed here, serialized or not.
        public Renderer targetTextureRenderer;
        public MeshFilter targetMeshFilter;
        public MeshRenderer targetMeshRenderer;
        public MeshCollider targetMeshCollider;

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
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        Vector3 scalingVector = new Vector3(-10.1f, 10.1f, 10.1f);

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        // Any Monobehaviour functions should be placed here.
        void Awake()
        {

        }

        void Start()
        {
            if (GetComponent<EndlessTerrainGenerator>() != null)
            {
                if (targetMeshRenderer != null) targetMeshRenderer.gameObject.SetActive(false);
                if (targetMeshFilter != null) targetMeshFilter.gameObject.SetActive(false);
                if (targetMeshCollider != null) targetMeshCollider.gameObject.SetActive(false);
                if (targetTextureRenderer != null) targetTextureRenderer.gameObject.SetActive(false);
            }
        }

        void Update()
        {

        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        // Any Non-Monobehaviour/custom functions should be placed here.
        // Note: Abstract/virtual functions or overrides have a separate section.
        public void DrawTexture(Texture2D texture)
        {
            if (targetMeshRenderer != null) targetMeshRenderer.gameObject.SetActive(false);
            if (targetMeshFilter != null) targetMeshFilter.gameObject.SetActive(false);
            if (targetMeshCollider != null) targetMeshCollider.gameObject.SetActive(false);

            if (targetTextureRenderer == null) return;

            targetTextureRenderer.gameObject.SetActive(true);

            targetTextureRenderer.sharedMaterial.mainTexture = texture;
            targetTextureRenderer.transform.localScale = new Vector3(texture.width / scalingVector.x, 1, texture.height / scalingVector.z);
        }

        public void DrawMesh(MeshData meshData, Texture2D texture)
        {
            if (targetTextureRenderer != null) targetTextureRenderer.gameObject.SetActive(false);

            if (targetMeshFilter == null || targetMeshRenderer == null || targetMeshCollider == null) return;

            targetMeshFilter.gameObject.SetActive(true);
            targetMeshRenderer.gameObject.SetActive(true);
            targetMeshCollider.gameObject.SetActive(true);

            Mesh generatedMesh = meshData.CreateMesh();
            targetMeshFilter.sharedMesh = generatedMesh;
            targetMeshRenderer.sharedMaterial.mainTexture = texture;

            if (targetMeshCollider != null)
            {
                targetMeshCollider.sharedMesh = null;
                targetMeshCollider.sharedMesh = generatedMesh;
            }
        }

        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
        // Any Abstract/virtual functions or overrides should be placed here.

    }
}