using UnityEngine;
using UnityEditor;
using System.IO;

namespace BarnoGames.Utilities
{
#if UNITY_EDITOR
    [CustomEditor(typeof(JsonSaveAndLoadScript))]
    public class SaveAndLoadEditor : Editor
    {
        private string _path;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            JsonSaveAndLoadScript jsonSaveAndLoadScript = (JsonSaveAndLoadScript)target;

            GUILayout.Space(20);
            GUI.color = Color.white;
            EditorGUILayout.LabelField("Save & Loading", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Folder Path"))
            {
                //_path = Path.Combine(Application.persistentDataPath, FILENAME);

                //if (File.Exists(_path))
                //{

                //}
                EditorUtility.RevealInFinder($"{Application.persistentDataPath}");

            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}