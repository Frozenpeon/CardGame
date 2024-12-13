using Com.IsartDigital.F2P.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Tools
{
#if UNITY_EDITOR
    public class CustomWindow : EditorWindow
    {
        protected const string ASSET_EXTENSION = ".asset";
        protected const string GENERATE_BTN_TEXT = "GENERATE";
        protected const string NAME = "Name :";
        protected const string TYPE = "Type :";
        protected const string SEPARATION = "_";
        protected const int SPACE_BETWEEN_VAR = 10;
        protected const int TITLE_FONT_SIZE = 50;
        protected GUIStyle _TitleLabel;

        protected void DisplayTitle(string pTitle, uint pWindowSizeX)
        {
            _TitleLabel = new GUIStyle(EditorStyles.wordWrappedLabel);
            _TitleLabel.fontSize = TITLE_FONT_SIZE;
            _TitleLabel.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.Space(SPACE_BETWEEN_VAR * 2);
            EditorGUILayout.LabelField(pTitle, _TitleLabel);
            EditorGUILayout.LabelField(SEPARATION.Duplicate(pWindowSizeX));
        }
        protected string DisplayText(string lText, string pValue, int pSpace = SPACE_BETWEEN_VAR)
        {
            EditorGUILayout.Space(pSpace);
            return EditorGUILayout.TextField(lText, pValue);
        }
    }
#endif
}