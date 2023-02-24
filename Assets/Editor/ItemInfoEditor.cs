using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemInfo))]
public class ItemInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty itemTypesField = serializedObject.FindProperty("itemType");

        SerializedProperty itemType = itemTypesField.FindPropertyRelative("item");
        EditorGUILayout.PropertyField(itemType);

        SerializedProperty tireType = itemTypesField.FindPropertyRelative("tire");
        SerializedProperty weaponType = itemTypesField.FindPropertyRelative("weapon");

        switch (itemType.enumNames[itemType.enumValueIndex])
        {
            case "None":
                tireType.enumValueIndex = 0;
                weaponType.enumValueIndex = 0;
                break;
            case "Tire":
                EditorGUILayout.PropertyField(tireType);
                weaponType.enumValueIndex = 0;
                break;
            case "Weapon":
                EditorGUILayout.PropertyField(weaponType);
                tireType.enumValueIndex = 0;
                break;
        }

        if (itemType.enumValueIndex != 0)
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
