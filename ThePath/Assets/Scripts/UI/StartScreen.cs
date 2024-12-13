using Com.IsartDigital.F2P.LoadScene;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.IsartDigital.F2P.UI.Menu
{
    public class StartScreen : Menu
    {
        [SerializeField] private SceneEnum _FTUEScene = 0;
        [SerializeField] private SceneEnum _TitleCardScene = 0;

        private LoadingScreenManager _LoadingScreenManager => LoadingScreenManager.Instance;
        public void OnTapBtn()
        {
            if (PlayerData.ActualPlayerData.isFTUE)
            {
                _LoadingScreenManager.LoadScene(_FTUEScene);
            }
            else
            {
                _LoadingScreenManager.LoadScene(_TitleCardScene);
            }
        }
    }
}
