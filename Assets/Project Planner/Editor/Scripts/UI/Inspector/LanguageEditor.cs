using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ProjectPlanner
{
    [CustomEditor(typeof(Language))]
    public class InspectorEditor : Editor
    {
        [SerializeField]
        private string search = string.Empty;
        [SerializeField]
        private string[] excluding = new string[0];

        public void OnEnable()
        {
            UpdateList();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language.Search, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent(Language.Search)).x));
            EditorGUI.BeginChangeCheck();
            search = EditorGUILayout.TextField(search);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateList();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            UI.Line();
            GUILayout.Space(10);
            

            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, excluding);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateList()
        {
            List<string> list = new List<string>();

            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.name == "m_Script")
                    {
                        list.Add(prop.name);
                    }
                    if (!string.IsNullOrEmpty(search) && !prop.displayName.ToLower().Contains(search.ToLower()))
                    {
                        list.Add(prop.name);
                    }
                }
                while (prop.NextVisible(false));
            }

            excluding = list.ToArray();
        }
    }
}