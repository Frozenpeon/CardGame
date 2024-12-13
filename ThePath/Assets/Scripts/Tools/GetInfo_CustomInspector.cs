using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Com.IsartDigital.F2P.Analytics
{
#if UNITY_EDITOR
    [CustomEditor(typeof(GetInfo))]
    public class GetInfo_CustomInspector : Editor
    {
        private int _Index = 0;
        private AnalyticsEventSO[] _Events;
        private const string PATH = "ScriptableObject/Analytics_Event/";
        private GetInfo _GetInfo => (GetInfo)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Object[] lEvents = Resources.LoadAll(PATH,typeof(AnalyticsEventSO));
            _Events = lEvents.OfType<AnalyticsEventSO>().ToArray();
            string[] lEventsName = new string[_Events.Length];

            for (int i = 0; i < _Events.Length; i++) lEventsName[i] = _Events[i].name;

            _Index = _GetInfo.index;
            _Index = EditorGUILayout.Popup("Tracker : ", _Index, lEventsName);

            _GetInfo.eventData = _Events[_Index].eventData;
            _GetInfo.index = _Index;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_GetInfo.eventData)));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}