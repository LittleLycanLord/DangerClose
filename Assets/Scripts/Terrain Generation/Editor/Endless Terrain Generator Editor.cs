using UnityEngine;
using UnityEditor;
using LilLycanLord_Official;

namespace LilLycanLord_Official
{
    [CustomEditor(typeof(EndlessTerrainGenerator))]
    public class EndlessTerrainGeneratorEditor : Editor
    {
        private const string ConstrainScalePrefKey = "EndlessTerrainGeneratorEditor.ConstrainTerrainScale";
        private bool constrainScale;

        private void OnEnable()
        {
            constrainScale = EditorPrefs.GetBool(ConstrainScalePrefKey, false);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.name == "m_Script")
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(iterator, true);
                    GUI.enabled = true;
                    continue;
                }

                if (iterator.name == "terrainScale")
                {
                    DrawConstrainedTerrainScale(iterator.Copy());
                    continue;
                }

                EditorGUILayout.PropertyField(iterator, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConstrainedTerrainScale(SerializedProperty terrainScaleProperty)
        {
            Vector3 oldScale = terrainScaleProperty.vector3Value;

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            Vector3 newScale = EditorGUILayout.Vector3Field(terrainScaleProperty.displayName, oldScale);
            if (EditorGUI.EndChangeCheck())
            {
                if (constrainScale)
                {
                    int changedAxis = GetChangedAxis(oldScale, newScale);
                    float oldAxisValue = GetAxisValue(oldScale, changedAxis);
                    float newAxisValue = GetAxisValue(newScale, changedAxis);

                    if (!Mathf.Approximately(oldAxisValue, 0f))
                    {
                        float scaleMultiplier = newAxisValue / oldAxisValue;
                        newScale = oldScale * scaleMultiplier;
                    }
                    else
                    {
                        newScale = new Vector3(newAxisValue, newAxisValue, newAxisValue);
                    }
                }

                terrainScaleProperty.vector3Value = newScale;
            }

            GUIContent lockIcon = EditorGUIUtility.IconContent(constrainScale ? "LockIcon-On" : "LockIcon");
            lockIcon.tooltip = "Constrain Terrain Scale Proportions";
            bool newConstrainScale = GUILayout.Toggle(constrainScale, lockIcon, GUI.skin.button, GUILayout.Width(24f), GUILayout.Height(EditorGUIUtility.singleLineHeight));

            if (newConstrainScale != constrainScale)
            {
                constrainScale = newConstrainScale;
                EditorPrefs.SetBool(ConstrainScalePrefKey, constrainScale);
            }

            EditorGUILayout.EndHorizontal();
        }

        private int GetChangedAxis(Vector3 oldValue, Vector3 newValue)
        {
            float xDelta = Mathf.Abs(newValue.x - oldValue.x);
            float yDelta = Mathf.Abs(newValue.y - oldValue.y);
            float zDelta = Mathf.Abs(newValue.z - oldValue.z);

            if (xDelta >= yDelta && xDelta >= zDelta) return 0;
            if (yDelta >= xDelta && yDelta >= zDelta) return 1;
            return 2;
        }

        private float GetAxisValue(Vector3 value, int axis)
        {
            if (axis == 0) return value.x;
            if (axis == 1) return value.y;
            return value.z;
        }

    }
}