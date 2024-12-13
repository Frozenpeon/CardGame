using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SceneEnum))]
public class ScenesPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw enum popup
        property.enumValueIndex = EditorGUI.Popup(position, label.text, property.enumValueIndex, property.enumDisplayNames);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(List<SceneEnum>))]
public class ScenesListPropertyDrawer : PropertyDrawer
{
    private ReorderableList reorderableList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (reorderableList == null)
        {
            reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true);
            reorderableList.drawElementCallback = DrawElement;
            reorderableList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, label);
        }

        reorderableList.DoList(position);
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rect, element, GUIContent.none);
    }
}
