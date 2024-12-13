using Com.IsartDigital.F2P.Analytics;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Tools.AnalyticsEventGenerator
{
#if UNITY_EDITOR
    public class AnalyticsEventGenerator : CustomWindow
    {
        private static AnalyticsEventGenerator _Window;

        private static Vector2 _WindowMaxSize = new Vector2(1000, 1000);
        private static Vector2 _WindowMinSize = new Vector2(50, 50);
        private const string WINDOW_NAME = "Analytics Event Generator";
        private const string PATH = "Assets/Resources/ScriptableObject/Analytics_Event/";
        private const string EVENT_NAME = "Event Name";
        private const string PARAMETERS = "Parameters :";
        private const string REMOVE = "Remove";
        private const string ADD = "Add New Parameter";

        private EventData _EventData = new EventData();
        private ParametersData _Parameter;

        private string _EventName = "";
        private const string TITLE = "EVENT GENERATOR";
        private int _ParametersCount => _EventData.parameters.Count;

        [MenuItem("Tools/EventGenerator")]
        public static void ShowWindow()
        {
            _Window = GetWindow<AnalyticsEventGenerator>(WINDOW_NAME);
            _Window.minSize = _WindowMinSize;
            _Window.maxSize = _WindowMaxSize;
        }

        private void OnEnable()
        {
            _EventData.parameters = new List<ParametersData>();
        }
        private void OnGUI()
        {
            DisplayTitle(TITLE, (uint)_Window.maxSize.x);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);

            _EventName = DisplayText(EVENT_NAME, _EventName);
            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            EditorGUILayout.LabelField(PARAMETERS);
            
            for (int i = 0; i < _ParametersCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _Parameter = _EventData.parameters[i];

                _Parameter.paramName = DisplayText(NAME, _Parameter.paramName, 0);
                GUILayout.FlexibleSpace();

                _Parameter.paramType = (ObjectType)EditorGUILayout.EnumPopup(TYPE, _Parameter.paramType);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(REMOVE))
                {
                    _EventData.parameters.RemoveAt(i);
                }                    

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(SPACE_BETWEEN_VAR);
            if (GUILayout.Button(ADD))
            {
                _EventData.parameters.Add(new ParametersData());
            }


            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GENERATE_BTN_TEXT))
            {
                CreateEvent();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                _EventData = new EventData();
                _EventData.parameters = new List<ParametersData>();
                _EventName = string.Empty;
            }
            GUI.enabled = true;
        }
        private void CreateEvent()
        {
            AnalyticsEventSO lEvent = CreateInstance<AnalyticsEventSO>();
            _EventData.eventName = _EventName;
            lEvent.eventData = _EventData;
            AssetDatabase.CreateAsset(lEvent, PATH + lEvent.eventData.eventName + ASSET_EXTENSION);
        }
    }
#endif
}