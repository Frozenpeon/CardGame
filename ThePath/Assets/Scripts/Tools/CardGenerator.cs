using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections.Generic;
using Com.IsartDigital.F2P.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Tools.CardGenerator
{

#if UNITY_EDITOR
    public class CardGenerator : CustomWindow
    {
        #region WINDOW'S VARIABLES
        private static CardGenerator _Window;

        private static Vector2 _WindowMaxSize = new Vector2(1000, 1000);
        private static Vector2 _WindowMinSize = new Vector2(50, 50);
        private static Vector2 _TextureSize = new Vector2(70, 70);

        private string _LabelID = "";
        private int _LabelIDFontSize = 12;
        #endregion

        #region CONSTANT
        private const string WINDOW_NAME = "Cards Generator";
        private string _Path = "";
        private const string PATH_NORMAL = "Assets/Resources/ScriptableObject/Card/Cards/";
        private const string PATH_MONSTER = "Assets/Resources/ScriptableObject/Card/Monsters/";
        private const string PATH_BOSS = "Assets/Resources/ScriptableObject/Card/Boss/";
        private const string CARD_DESCRIPTION = "Description : ";
        private const string CARD_IMAGE = "Image :";
        private const string CARD_EFFECT = "Effect :";
        private const string CARD_LIFE_REMOVED = "Life Removed :";
        private const string CARD_ATTACK_REMOVED = "Attack Removed :";
        private const string CARD_WHEAT_REMOVED = "Wheat Removed :";
        private const string CARD_EFFECT_VALUE = "Effect Value :";
        private const string CARD_POWER = "Sharpening :";
        private const string SPECIAL_BOOST = "Special Boost :";
        private const string CARD_AFFECTED = "Card Affected :";
        private const string CARD_IS_BOSS = "Is Boss :";
        private const string CARD_MONSTER_LEVEL = "Level :";
        private const string TITLE = "CARD GENERATOR";
        private const string CATEG_COMMUN = "Commun Card's Variables";
        private const string CATEG_NORMAL = "Normal Card's Variables";
        private const string CATEG_MONSTER = "Monster Card's Variables";
        private const string CATEG_BOOST = "Boost Card's Variables";
        private const int CATEG_SPACING = -57;
        private const int CATEG_LEFT_MARGIN = 40;
        private const int CATEG_FONT_SIZE = 20;
        #endregion

        #region CARD'S VARIABLES

        // All card : 
        private static int _TotalID = 0;
        private string _CardName;
        private string _CardDescription;
        private Sprite _CardImage;
        private CardType _CardType = CardType.CropField;
        private int _CardEffectValue;
        private CardEffectType _CardTypeEffect;

        // Monster : 
        private bool _IsBoss;
        private int _CardPower;
        private int _WheatRemoved;
        private int _AttackRemoved;
        private int _LifeRemoved;
        private int _Level;
        private const int MONSTER_START_LEVEL = 1;
        private const int MONSTER_END_LEVEL = 999;

        // Boost : 
        private CardAffected _CardAffected;
        private SpecialBoost _SpecialBoost;
        private bool _IDFound;
        #endregion

        private Color _CategColor = Color.white;
        private GUIStyle _CategStyle;

        private List<List<string>> _Prefix = new List<List<string>>()
        {
            new List<string>() { "Coward", "nKid","Small","Weak" },
            new List<string>() { "Spooky", "Young", "Insolent", "Insolent" },
            new List<string>() { "Scary", "Strong", "Tough", "Bold" },
            new List<string>() { "Spooky", "Young", "Insolent", "Insolent" },
            new List<string>() { "Big", "Brutal", "Powerfull", "Vicious" },
            new List<string>() { "Terrifying", "Horrific", "King", "Huge", "Infernal", "Demoniac", "Colossal", "Deadly" }
        };
        private int _IndexPrefix = 0;
        private int _IndexPrefixInList = 0;
        private string _PrefixName = "";

        [MenuItem("Tools/CardsGenerator")]
        public static void ShowWindow()
        {
            _Window = GetWindow<CardGenerator>(WINDOW_NAME);
            _Window.minSize = _WindowMinSize;
            _Window.maxSize = _WindowMaxSize;
        }
        private void OnGUI()
        {
            DisplayTitle(TITLE, (uint)_Window.maxSize.x);
            _Path = _CardType != CardType.Monster ? PATH_NORMAL : (_IsBoss ? PATH_BOSS : PATH_MONSTER);
            DisplayCategLabel();

            _CardName = DisplayText(NAME, _CardName);
            _CardDescription = DisplayText(CARD_DESCRIPTION, _CardDescription);

            EditorGUILayout.Space(SPACE_BETWEEN_VAR);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(CARD_IMAGE);
            GUILayout.Space(CATEG_SPACING);
            _CardImage = (Sprite)EditorGUILayout.ObjectField(_CardImage, typeof(Texture2D), false, GUILayout.Width(_TextureSize.x), GUILayout.Height(_TextureSize.y));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            _CardType = DisplayEnum(TYPE, _CardType, SPACE_BETWEEN_VAR * 2);

            EditorGUILayout.Space(SPACE_BETWEEN_VAR);

            switch (_CardType)
            {
                case CardType.Monster:
                    DisplayLabel(CATEG_MONSTER);
                    break;
                case CardType.Boost:
                    DisplayLabel(CATEG_BOOST);
                    break;
                default:
                    DisplayLabel(CATEG_NORMAL);
                    break;
            }

            // ----- Monster ----- \\
            if (_CardType == CardType.Monster)
            {
                EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                _IsBoss = EditorGUILayout.Toggle(CARD_IS_BOSS, _IsBoss);

                _Level = Mathf.Clamp(DisplayInt(CARD_MONSTER_LEVEL, _Level), MONSTER_START_LEVEL, MONSTER_END_LEVEL);
                _IndexPrefix = Mathf.Clamp(_Level - 1, 1, _Prefix.Count - 1);
                string[] lEventsName = new string[_Prefix[_IndexPrefix].Count];

                for (int i = 0; i < lEventsName.Length; i++) lEventsName[i] = _Prefix[_IndexPrefix][i];
                _IndexPrefixInList = Random.Range(0, _Prefix[_IndexPrefix].Count);

                if (_IsBoss) UpdateTotalID(PATH_BOSS);
                else UpdateTotalID(PATH_MONSTER);

                _CardPower = DisplayInt(CARD_POWER, _CardPower);

                _WheatRemoved = DisplayInt(CARD_WHEAT_REMOVED, _WheatRemoved);
                _AttackRemoved = DisplayInt(CARD_ATTACK_REMOVED, _AttackRemoved, 0);
                _LifeRemoved = DisplayInt(CARD_LIFE_REMOVED, _LifeRemoved, 0);
            }
            else
            {
                UpdateTotalID(PATH_NORMAL);

                _CardTypeEffect = DisplayEnum(CARD_EFFECT, _CardTypeEffect);

                EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                _CardEffectValue = EditorGUILayout.IntField(CARD_EFFECT_VALUE, _CardEffectValue);

                if (_CardType == CardType.Boost)
                {
                    _CardAffected = DisplayEnum(CARD_AFFECTED, _CardAffected);

                    if (_CardTypeEffect == CardEffectType.Other)
                        _SpecialBoost = DisplayEnum(SPECIAL_BOOST, _SpecialBoost);
                }
            }

            GUILayout.FlexibleSpace();
            GUI.enabled = _IDFound;
            if (GUILayout.Button(GENERATE_BTN_TEXT))
            {
                switch (_CardType)
                {
                    case CardType.Monster:
                        SetCard<MonsterSO>(_CardType);
                        break;
                    case CardType.Boost:
                        SetCard<BoostSO>(_CardType);
                        break;
                    default:
                        SetCard<CardSO>(_CardType);
                        break;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.enabled = true;

            DisplayBottomLabel();
        }

        private void DisplayCategLabel()
        {
            _CategStyle = new GUIStyle();
            _CategStyle.fontSize = CATEG_FONT_SIZE;
            _CategStyle.margin.left = CATEG_LEFT_MARGIN;
            _CategStyle.normal.textColor = _CategColor;
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            EditorGUILayout.LabelField(CATEG_COMMUN, _CategStyle);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
        }

        private void DisplayLabel(string pTitle)
        {
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            EditorGUILayout.LabelField(pTitle, _CategStyle);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
        }

        private TEnum DisplayEnum<TEnum>(string lText, TEnum pEnum, int pSpace = SPACE_BETWEEN_VAR) where TEnum : System.Enum
        {
            EditorGUILayout.Space(pSpace);
            return (TEnum)EditorGUILayout.EnumPopup(label: lText, selected: pEnum);
        }
        private int DisplayInt(string lText, int pValue, int pSpace = SPACE_BETWEEN_VAR)
        {
            EditorGUILayout.Space(pSpace);
            return EditorGUILayout.IntField(label: lText, value: pValue);
        }
        
        private void DisplayBottomLabel()
        {
            //Author : Julian Martin
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _LabelID = _IDFound ? $"Your ID is: {_TotalID}" : "Searching ID";
            EditorGUILayout.LabelField(_LabelID, new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = _LabelIDFontSize });

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void SetCard<T>(CardType pType) where T : CardSO
        {
            T lCard = CreateInstance<T>();
            lCard.ID = _TotalID;
            lCard.cardName = _CardName;
            lCard.description = _CardDescription;
            lCard.effectValue = _CardEffectValue;
            lCard.image = _CardImage;
            lCard.cardType = pType;

            switch (_CardType)
            {
                case CardType.Monster:
                    (lCard as MonsterSO).power = _CardPower;
                    (lCard as MonsterSO).wheatRemoved = _WheatRemoved;
                    (lCard as MonsterSO).attackRemoved = _AttackRemoved;
                    (lCard as MonsterSO).lifeRemoved = _LifeRemoved;
                    (lCard as MonsterSO).level = _Level;
                    (lCard as MonsterSO).prefix = _Prefix[_IndexPrefix][_IndexPrefixInList];
                    break;

                case CardType.Boost:
                    (lCard as BoostSO).cardAffected = _CardAffected;
                    (lCard as BoostSO).effectType = _CardTypeEffect;
                    (lCard as BoostSO).specialBoost = _SpecialBoost;
                    break;
            }

            AssetDatabase.CreateAsset(lCard, _Path + lCard.cardName + ASSET_EXTENSION);
        }

        private void UpdateTotalID(string pFolderToSearchIn)
        {
            //Author : Julian Martin

            _IDFound = false;
            // Fetch all assets of type CardSO from the path
            string[] assetGUIDs = AssetDatabase.FindAssets("t:CardSO", new string[] { pFolderToSearchIn });
            List<int> existingIDs = new List<int>();

            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                CardSO card = AssetDatabase.LoadAssetAtPath<CardSO>(assetPath);
                if (card != null)
                {
                    existingIDs.Add(card.ID);
                }
            }

            // Find the first missing ID in a sorted list of existing IDs
            existingIDs.Sort();
            int missingID = 0; // Start checking from ID 1
            foreach (int id in existingIDs)
            {
                if (id != missingID)
                    break;
                missingID++;
            }

            _TotalID = missingID; // Set the _TotalID to the first missing ID found
            _IDFound = true;
        }
    }
#endif
}