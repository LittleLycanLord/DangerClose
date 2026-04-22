using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
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
        // #NOTTRIM#
        // }

        // void Start() 
        // {
        // #NOTTRIM#
        // }

        // void Update() 
        // {
        // #NOTTRIM#
        // }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        // Any Non-Monobehaviour/custom functions should be placed here.
        // Note: Abstract/virtual functions or overrides have a separate section.
        public override void OnInspectorGUI()
        {
            MapGenerator generator = (MapGenerator)target;
            if (DrawDefaultInspector())
            {
                generator.DrawMapInEditor();
            }

            if (GUILayout.Button("Generate Map"))
            {
                generator.DrawMapInEditor();
            }
        }

        //* ╔════════════════════════════════╗
        //* ║ Virtual / Overridden Functions ║
        //* ╚════════════════════════════════╝
	    // Any Abstract/virtual functions or overrides should be placed here.

    }
}