using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        DataManager dataManager = (DataManager)target;

        if (GUILayout.Button("Open Saves Folder"))
        {
            dataManager.OpenSavesFolder();
        }
    }
}
