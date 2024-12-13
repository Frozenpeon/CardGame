using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Com.IsartDigital.F2P.LoadScene;

namespace Com.IsartDigital.F2P.UI.Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] protected Canvas[] _UnshowGameElements = default;
        protected static Action OnBtnSettingsClick;
        [SerializeField] protected int m_NextSceneIndex = 0;
        protected bool _OpenSettings = false;

        public void ChangeScene() => LoadingScreenManager.Instance.LoadScene((SceneEnum)m_NextSceneIndex);
        public void ChangeScene(int pSceneIndex) => LoadingScreenManager.Instance.LoadScene((SceneEnum)pSceneIndex);
        public virtual void OnSettingsBtn()
        {
            UpdateSettings();
            OnBtnSettingsClick();
        }
        public void UpdateSettings()
        {
            UpdateGameElementsCanvas(_OpenSettings);
            _OpenSettings = !_OpenSettings;
        }
        protected void UpdateGameElementsCanvas(bool pState)
        {
            foreach (Canvas lElement in _UnshowGameElements)
            {
                lElement.enabled = pState;
            }
        }
    }
}