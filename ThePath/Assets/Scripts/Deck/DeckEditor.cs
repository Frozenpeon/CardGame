using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Com.IsartDigital.F2P;

#if UNITY_EDITOR
[CustomEditor(typeof(Deck))]
public class DeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Load the current values from the target into the serializedObject

        Deck deck = (Deck)target;

        // Manually draw the 'isFTUE' toggle
        bool newIsFTUE = EditorGUILayout.Toggle("Is FTUE", deck.isFTUE);
        if (newIsFTUE != deck.isFTUE)
        {
            Undo.RecordObject(deck, "Toggle Is FTUE");
            deck.isFTUE = newIsFTUE;
            EditorUtility.SetDirty(deck);
        }

        // Conditionally draw the 'cardConfigList'
        if (deck.isFTUE)
        {
            SerializedProperty cardConfigList = serializedObject.FindProperty("cardConfigList");
            EditorGUILayout.PropertyField(cardConfigList, new GUIContent("Card Configuration List"), true);
        }

        // Draw the default inspector minus 'isFTUE'
        DrawPropertiesExcluding(serializedObject, "m_Script", "isFTUE", "cardConfigList");
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
