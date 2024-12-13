using Com.IsartDigital.F2P.Game.FTUE;
using Com.IsartDigital.F2P.Manager;

namespace Com.IsartDigital.F2P.UI.Menu
{
    public class CollectionMenu : Menu
    {
        private UIManager _UIManager => UIManager.GetInstance;
        void Start()
        {
            if (_UIManager)
            {
                OnBtnSettingsClick += _UIManager.SwitchSettingsOverlay;
            }
        }

        private void OnDestroy()
        {
            if (_UIManager)
            {
                OnBtnSettingsClick -= _UIManager.SwitchSettingsOverlay;
            }
        }

        public void UpdateFTUE()
        {
            if (FTUEMenu.instance)
            {
                FTUEMenu.instance.ChangeUIFTUE();
            }
        }
    }
}