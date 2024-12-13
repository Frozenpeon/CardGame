using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.UI.HUD;
using Com.IsartDigital.F2P.UI.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI.Overlay
{
	public enum VolumeType
	{
		General,
		SFX
	}

	public class SettingsOverlay : MonoBehaviour
	{
		public static Action OnBtnBackClick;
		public static event Action<float, VolumeType> OnMusicVolumeChanged;
        private MenuPause _MenuPause => MenuPause.instance;

		private int _LanguageIndex = 0;

		[SerializeField] private List<Sprite> _LanguageImages = new List<Sprite>();
		[SerializeField] private Image _LanguageBtnImage = default;
		[SerializeField] private TextMeshProUGUI _PlayerName = default;

        private void OnEnable()
		{
			OnBtnBackClick += UIManager.GetInstance.SwitchSettingsOverlay;
            if(_MenuPause) OnBtnBackClick += _MenuPause.UpdateSettings;

            if (_PlayerName) _PlayerName.text = PlayerData.ActualPlayerData.PlayerUsernName;
            //OnMusicVolumeChanged += SoundManager.GetInstance.OnMusicVolumeChange;
        }

		private void OnDisable()
		{
			OnBtnBackClick = null;
		}

		void Start()
		{

		}

		/// <summary>
		/// Emit a signal that will play the SwitchSettingsOverlay function from UIManager
		/// </summary>
		public void OnBackBtn()
		{
			OnBtnBackClick();
		}

		public void OnLocalizationChanged()
		{
			_LanguageIndex++;
			int lTextureIndex = _LanguageIndex < Utils.Addons_Enum.GetEnumCount<SettingsManager.Language>() ? _LanguageIndex : 0;
			_LanguageBtnImage.sprite = _LanguageImages[lTextureIndex];

            if (_LanguageIndex > Utils.Addons_Enum.GetEnumCount<SettingsManager.Language>() -1) _LanguageIndex = 0;
            SettingsManager.OnSwitchLanguage(_LanguageIndex);
		}

		public void OnMusicVolumeChange(float pVolume, VolumeType pType)
		{
			OnMusicVolumeChanged(pVolume/100, pType);
		}
	}
}