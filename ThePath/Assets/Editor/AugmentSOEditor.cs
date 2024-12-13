
using Com.IsartDigital.F2P.SO.CardSO;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    [CustomEditor(typeof(AugmentSOTrade))]
    public class NewBehaviourScript : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AugmentSOTrade m_augmentSO = (AugmentSOTrade)target;
            // Draw offeredItem field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("augmentName"));
            // Draw offeredItem field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("augmentDescription"));

            // Draw offeredItem field
            EditorGUILayout.LabelField("Offered Item", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offeredItem"));
            serializedObject.ApplyModifiedProperties();
            if (m_augmentSO.offeredItem == TradedItem.card)
            {
                m_augmentSO.cardTypeOffered = (CardType)EditorGUILayout.EnumPopup("Card type offered", m_augmentSO.cardTypeOffered);

            } else if (m_augmentSO.offeredItem == TradedItem.ressources)
            {
                m_augmentSO.ressourceTypeOffered = (Ressources)EditorGUILayout.EnumPopup("Card type offered", m_augmentSO.ressourceTypeOffered);

            }         
            // Draw amountOffered field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("amountOffered"));
            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();


            EditorGUILayout.LabelField("Received Item", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            // Draw receivedItemType field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("receivedItem"));
            
            if (m_augmentSO.receivedItem == TradedItem.card)
            {
                m_augmentSO.cardTypeReceived = (CardType)EditorGUILayout.EnumPopup("Card type received", m_augmentSO.cardTypeReceived);
            }
            else if (m_augmentSO.receivedItem == TradedItem.ressources)
            {
                m_augmentSO.ressourceTypeReceived = (Ressources)EditorGUILayout.EnumPopup("Card type received", m_augmentSO.ressourceTypeReceived);
            }
            // Draw amountReceived field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("amountReceived"));
            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
      
    }
}
