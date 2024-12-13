using Com.IsartDigital.F2P.Manager;
using UnityEngine;

namespace Com.IsartDigital.F2P.UI.Menu
{
    public class TitleCard : Menu
    {
        private UIManager _UIManager => UIManager.GetInstance;
        private void Start()
        {
            if (_UIManager) _UIManager.SwitchToMainMenu();
        }
    }
}