using Com.IsartDigital.F2P.Manager;

namespace Com.IsartDigital.F2P.UI.Screen
{
    public class MainMenuScreen : Menu.Menu
    {	

		private void OnEnable()
		{
			OnBtnSettingsClick += UIManager.GetInstance.SwitchSettingsOverlay;
		}

		private void OnDisable()
        {
            OnBtnSettingsClick -= UIManager.GetInstance.SwitchSettingsOverlay;
            OnBtnSettingsClick = null;
		}

		// Start is called before the first frame update
		void Start()
		{

		}

		/// <summary>
		/// Emit a signal that will play the SwitchSettingsOverlay function from UIManager
		/// </summary>
		public override void OnSettingsBtn()
		{
			base.OnSettingsBtn();
            OnBtnSettingsClick();
		}


	}
}