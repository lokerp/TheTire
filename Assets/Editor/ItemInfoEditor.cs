using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemInfo))]
public class ItemInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty baseType = serializedObject.FindProperty("itemType");
        EditorGUILayout.PropertyField(baseType);

        SerializedProperty additiveType = null;

        switch (baseType.enumNames[baseType.enumValueIndex])
        {
            case "None":
                serializedObject.FindProperty("tireType").Reset();
                serializedObject.FindProperty("weaponType").Reset();
                break;
            case "Tire":
                additiveType = serializedObject.FindProperty("tireType");
                EditorGUILayout.PropertyField(additiveType);
                serializedObject.FindProperty("weaponType").enumValueIndex = 0;
                break;
            case "Weapon":
                additiveType = serializedObject.FindProperty("weaponType");
                EditorGUILayout.PropertyField(additiveType);
                serializedObject.FindProperty("tireType").enumValueIndex = 0;
                break;
        }

        if (additiveType != null && additiveType.enumValueIndex != 0)
            ShowCommonInfo();

        serializedObject.ApplyModifiedProperties();
    }

    void ShowCommonInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("properties"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cost"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("path"));
    }
}
