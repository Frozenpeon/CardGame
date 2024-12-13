using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Com.IsartDigital.F2P.SO;

namespace Com.IsartDigital.F2P.Tools.Booster
{
#if UNITY_EDITOR
    public class BoosterGenerator : EditorWindow
    {
        private static BoosterGenerator _Window;

        private static Vector2 _WindowMaxSize = new Vector2(1000, 1000);
        private static Vector2 _WindowMinSize = new Vector2(50, 50);

        #region CONSTANT
        private const string WINDOW_NAME = "Booster Generator";
        private const string PATH = "Assets/Resources/ScriptableObject/Booster/";
        private const string ASSET_EXTENSION = ".asset";

        private const string BOOSTER_NAME = "Name : ";
        private const string NUMBER_OF_CARDS = "Number of cards : ";
        private const string COMMON_DROP_RATE = "Common card drop rate :";
        private const string RARE_DROP_RATE = "Rare card drop rate :";
        private const string CARDS_LIST = "Cards List :";
        private const string GENERATE_BTN_TEXT = "GENERATE";

        private const int SPACE_BETWEEN_VAR = 10;
        #endregion

        private string _BoosterName;

        private int _NumberOfCards = 0;
        private int _CommonDropRate = 0;
        private int _RareDropRate = 0;

        private AugmentListSO _AugmentListSO;

        [MenuItem("Tools/BoosterGenerator")]
        public static void ShowWindow()
        {
            _Window = GetWindow<BoosterGenerator>(WINDOW_NAME);
            _Window.minSize = _WindowMinSize;
            _Window.maxSize = _WindowMaxSize;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            _BoosterName = EditorGUILayout.TextField(BOOSTER_NAME, _BoosterName);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            _NumberOfCards = EditorGUILayout.IntField(NUMBER_OF_CARDS, _NumberOfCards);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            _CommonDropRate = EditorGUILayout.IntField(COMMON_DROP_RATE, _CommonDropRate);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            _RareDropRate = EditorGUILayout.IntField(RARE_DROP_RATE, _RareDropRate);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            _AugmentListSO = EditorGUILayout.ObjectField(CARDS_LIST, _AugmentListSO, typeof(AugmentListSO), true) as AugmentListSO;

            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(GENERATE_BTN_TEXT))
            {
                SetBooster<BoosterSO>();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
        }

        private void SetBooster<T>() where T : BoosterSO
        {
            T lBooster = CreateInstance<T>();
            lBooster.cards = _AugmentListSO.AugmentList;
            lBooster.cardDropAmount = _NumberOfCards;
            lBooster.dropRates.Add(_CommonDropRate);
            lBooster.dropRates.Add(_RareDropRate);

            AssetDatabase.CreateAsset(lBooster, PATH + _BoosterName + ASSET_EXTENSION);
        }
    }
#endif
}
