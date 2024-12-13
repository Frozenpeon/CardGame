using Com.IsartDigital.F2P.SO.CardSO;
using Com.IsartDigital.F2P.SO.QuestSO;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//By Julian Martin
namespace Com.IsartDigital.F2P.Tools.QuestGenerator
{
#if UNITY_EDITOR
    public class QuestGenerator : EditorWindow
    {
        #region WINDOW'S VARIABLES
        private static QuestGenerator _Window;

        private static Vector2 _WindowMaxSize = new Vector2(1000, 1000);
        private static Vector2 _WindowMinSize = new Vector2(50, 50);
        private static Vector2 _TextureSize = new Vector2(70, 70);
        private Vector2 _scrollPosition;
        #endregion

        #region CONSTANT
        private const string WINDOW_NAME = "Quest Generator";

        //Paths
        private string _Path = "";
        private const string PATH_KILL_QUESTS = "Assets/Resources/ScriptableObject/Quest/Quests/KillQuests/";
        private const string PATH_PLAY_QUESTS = "Assets/Resources/ScriptableObject/Quest/Quests/PlayQuests/";
        private const string PATH_CARDS_QUESTS = "Assets/Resources/ScriptableObject/Quest/Quests/CardsQuests/";
        private const string PATH_CARDS_FOLDER = "Assets/Resources/ScriptableObject/Card/Cards/";

        //Quest variables
        private const string QUEST_IMAGE = "Image :";
        private const string QUEST_TYPE = "Type :";
        private const string QUEST_DURATION = "Duration :";
        private const string START_TIME = "Start time :";

        //Enemies related
        private const string ENEMY_TYPE = "Enemy type :";
        private const string KILL_QUANTITY = "Enemies amount to kill :";

        //Play related
        private const string PLAY_TYPE = "Play type :";
        private const string GAMES_QUANTITY = "Quantity to play :";

        //Cards related
        private const string CARDS_TYPE = "Cards type :";
        private const string CARD_TO_PLAY = "Card to play :";
        private const string CARD_AMOUNT_TO_PLAY = "Amount to play :";

        //Other
        private const string REWARDS = "Rewards :";
        private const string GENERATE_BTN_TEXT = "GENERATE";
        private const string ASSET_EXTENSION = ".asset";
        private const int SPACE_BETWEEN_VAR = 10;

        //For description
        private const string KILL_ACTION = "Kill";
        private const string MONSTER_SUFFIX = "monster";
        private const string MONSTERS_SUFFIX = "monsters";
        private const string BOSS_SUFFIX = "boss";
        private const string BOSSES_SUFFIX = "bosses";
        private const string PLAY_ACTION = "Play";
        private const string CARDS_SUFFIX = "cards";
        private const string CARD_SUFFIX = "card";
        private const string COMPLETE_ACTION = "Complete";
        private const string TURNS_SUFFIX = "turns";
        private const string GAMES_SUFFIX = "games";
        #endregion

        #region QUEST'S VARIABLES

        // All quest : 
        private static int _TotalID = 0;
        private string _QuestName;
        private string _QuestDescription;
        private Texture2D _QuestImage;
        private QuestType _QuestType;
        private int _ID;

        private bool _IDFound;
        //Enemies
        private EnemiesType _Enemy;
        private int _KillQuantity;

        //Plays
        private PlayType _Play;
        private int _PlayQuantity;

        //Cards
        private CardsType _Cards;
        private CardSO _CardSO;
        private int _NumberOfCardToPlay;
        #endregion


        [MenuItem("Tools/QuestGenerator")]
        public static void ShowWindow()
        {
            _Window = GetWindow<QuestGenerator>(WINDOW_NAME);
            _Window.minSize = _WindowMinSize;
            _Window.maxSize = _WindowMaxSize;
        }
        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            EditorGUILayout.LabelField(QUEST_IMAGE, new GUIStyle(EditorStyles.boldLabel));
            
            EditorGUILayout.Space(SPACE_BETWEEN_VAR * 2);
            _QuestImage = (Texture2D)EditorGUILayout.ObjectField(_QuestImage, typeof(Texture2D), false, 
                                                                   GUILayout.Width(_TextureSize.x),
                                                                   GUILayout.Height(_TextureSize.y));

            EditorGUILayout.Space(SPACE_BETWEEN_VAR * 2);
            _QuestType = (QuestType)EditorGUILayout.EnumPopup(QUEST_TYPE, _QuestType);

            switch (_QuestType)
            {
                case QuestType.Kill:
                    _Path = PATH_KILL_QUESTS;
                    EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                    _Enemy = (EnemiesType)EditorGUILayout.EnumPopup(ENEMY_TYPE, _Enemy);

                    EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                    _KillQuantity = EditorGUILayout.IntField(KILL_QUANTITY, _KillQuantity);
                    break;
                case QuestType.Play:
                    _Path = PATH_PLAY_QUESTS;
                    EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                    _Play = (PlayType)EditorGUILayout.EnumPopup(PLAY_TYPE, _Play);

                    EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                    _PlayQuantity = EditorGUILayout.IntField(GAMES_QUANTITY, _PlayQuantity);
                    break;
                case QuestType.Cards:
                    _Path = PATH_CARDS_QUESTS;
                    EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                    _Cards = (CardsType)EditorGUILayout.EnumPopup(CARDS_TYPE, _Cards);

                    EditorGUILayout.Space(SPACE_BETWEEN_VAR);
                    //_CardSO = (CardSO)EditorGUILayout.ObjectField(CARD_TO_PLAY, _CardSO, typeof(CardSO), false);
                    SelectCardSO();
                    _NumberOfCardToPlay = EditorGUILayout.IntField(CARD_AMOUNT_TO_PLAY, _NumberOfCardToPlay);
                    break;
                default:
                    break;
            }

            UpdateTotalID(_Path);
           

            GUILayout.FlexibleSpace(); // Pushes the button to the bottom
            GUI.enabled = _IDFound/* && ValidateQuestCreation()*/;
            if (GUILayout.Button(GENERATE_BTN_TEXT))
            {
                switch (_QuestType)
                {
                    case QuestType.Kill:
                        SetQuest<KillQuestSO>(_QuestType);
                        break;
                    case QuestType.Play:
                        SetQuest<PlayQuestSO>(_QuestType);
                        break;
                    case QuestType.Cards:
                        SetQuest<CardQuestSO>(_QuestType);
                        break;
                    default: break;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.enabled = true;

            DisplayBottomLabel();
            EditorGUILayout.EndScrollView();
        }

        private void SelectCardSO()
        {
            string lFolderPath = PATH_CARDS_FOLDER; // Adjust path as necessary
                                                   // Get all CardSO assets in the specified folder
            string[] lGuids = AssetDatabase.FindAssets("t:CardSO", new[] { lFolderPath });
            List<CardSO> lCards = new List<CardSO>();
            List<string> lCardNames = new List<string>();

            foreach (string lGuid in lGuids)
            {
                string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuid);
                CardSO lCard = AssetDatabase.LoadAssetAtPath<CardSO>(lAssetPath);
                if (lCard != null)
                {
                    lCards.Add(lCard);
                    lCardNames.Add(lCard.name);
                }
            }

            if (lCards.Count > 0)
            {
                int lCurrentIndex = lCards.IndexOf(_CardSO);
                int lSelectedIndex = EditorGUILayout.Popup(CARD_TO_PLAY, lCurrentIndex, lCardNames.ToArray());
                if (lCurrentIndex != lSelectedIndex) // Check if selection has changed
                {
                    _CardSO = lCards[lSelectedIndex];
                }
            }
            else
            {
                EditorGUILayout.LabelField("No cards found in the specified folder.");
            }
        }


        private bool ValidateQuestCreation(string pPotentialDuplicatePath)
        {
            // Check if a quest with the same name already exists at the path
            if (AssetDatabase.LoadAssetAtPath<QuestSO>(pPotentialDuplicatePath) != null)
            {
                EditorUtility.DisplayDialog("Validation Error", "This quest already exists.", "OK");
                return false;
            }

            return true;  // No errors found
        }

        private void DisplayBottomLabel()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            string label = _IDFound ? $"Your ID is: {_TotalID}" : "Searching ID";
            EditorGUILayout.LabelField(label, new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 12 });

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void SetQuest<T>(QuestType pType) where T : QuestSO
        {
            if (pType == QuestType.Kill && _KillQuantity < 1)
            {
                EditorUtility.DisplayDialog("Invalid Input", "Kill quantity must be at least 1.", "OK");
                return;
            }
            else if (pType == QuestType.Play && _PlayQuantity < 1)
            {
                EditorUtility.DisplayDialog("Invalid Input", "Quantity to play must be at least 1.", "OK");
                return;
            }
            else if (pType == QuestType.Cards)
            {
                if (_CardSO == null)
                {
                    EditorUtility.DisplayDialog("Invalid Input", "Please select a card to play.", "OK");
                    return;
                }
                if (_NumberOfCardToPlay < 1)
                {
                    EditorUtility.DisplayDialog("Invalid Input", "Amount to play must be at least 1.", "OK");
                    return;
                }
            }

            T lQuest = CreateInstance<T>();
            lQuest.ID = _TotalID;
            lQuest.questType = pType;

            switch (_QuestType)
            {
                case QuestType.Kill:
                    lQuest.questName = $"{KILL_ACTION}{_KillQuantity}{_Enemy}";
                    switch (_Enemy)
                    {
                        case EnemiesType.Monster:
                            lQuest.description = $"{KILL_ACTION} {_KillQuantity} {(_KillQuantity > 1 ? MONSTERS_SUFFIX : MONSTER_SUFFIX)}";
                            break;
                        case EnemiesType.Boss:
                            lQuest.description = $"{KILL_ACTION} {_KillQuantity} {(_KillQuantity > 1 ? BOSSES_SUFFIX : BOSS_SUFFIX)}";
                            break;
                    }
                    (lQuest as KillQuestSO).enemyType = _Enemy;
                    (lQuest as KillQuestSO).killAmount = _KillQuantity;
                    break;

                case QuestType.Cards:
                    lQuest.questName = $"{PLAY_ACTION}{_NumberOfCardToPlay}{_CardSO.name}";
                    lQuest.description = $"{PLAY_ACTION} {_NumberOfCardToPlay} {_CardSO.name} {(_NumberOfCardToPlay > 1 ? CARDS_SUFFIX : CARD_SUFFIX)}";
                    (lQuest as CardQuestSO).cardToPlay = _CardSO;
                    (lQuest as CardQuestSO).numberOfCardToPlay = _NumberOfCardToPlay;
                    break;
                case QuestType.Play:
                    string lPlaySuffix = _Play == PlayType.Turn ? TURNS_SUFFIX : GAMES_SUFFIX;
                    lQuest.questName = $"{COMPLETE_ACTION}{_PlayQuantity}{lPlaySuffix}";
                    lQuest.description = $"{COMPLETE_ACTION} {_PlayQuantity} {lPlaySuffix}";
                    (lQuest as PlayQuestSO).playType = _Play;
                    (lQuest as PlayQuestSO).numberOfPlaysRequired = _PlayQuantity;
                    break;
            }

            string potentialDuplicatePath = _Path + lQuest.questName + ASSET_EXTENSION;
            if (ValidateQuestCreation(potentialDuplicatePath))
            {
                AssetDatabase.CreateAsset(lQuest, _Path + lQuest.questName + ASSET_EXTENSION);
            }
        }

        private void UpdateTotalID(string pFolderToSearchIn)
        {
            _IDFound = false;
            string[] assetGUIDs = AssetDatabase.FindAssets("t:QuestSO", new string[] { pFolderToSearchIn });
            List<int> existingIDs = new List<int>();

            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                QuestSO lQuest = AssetDatabase.LoadAssetAtPath<QuestSO>(assetPath);
                if (lQuest != null)
                {
                    existingIDs.Add(lQuest.ID);
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