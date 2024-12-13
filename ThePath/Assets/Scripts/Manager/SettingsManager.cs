using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Com.IsartDigital.F2P.Manager
{
	public static class SettingsManager
	{
		private static Language _currentLanguage = Language.English;

		public static Action<int> OnSwitchLanguage;

		public enum Language
		{
			English,
			French
		}

		public static Language GetLanguage
		{
			get { return _currentLanguage; }
			private set { _currentLanguage = value; }
		}

		/// <summary>
		/// Function to play on the first frames of the game that will allow to manage settings of the game like language or volume.
		/// </summary>
		public static void SettingsInit()
		{
			SwitchLanguage(0);

			ConnectAllEvents();
		}

		private static void ConnectAllEvents()
		{
			OnSwitchLanguage += SwitchLanguage;
		}

		/// <summary>
		/// Modify the current language of the game by the given parameters value
		/// </summary>
		/// <param name="pLanguageId">Integer value of the language enums</param>
		private static void SwitchLanguage(int pLanguageId)
		{
			_currentLanguage = (Language)pLanguageId;
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[pLanguageId];
		}


		public static void SettingsKill()
		{
			DisconnectAllEvents();
		}

		private static void DisconnectAllEvents()
		{
			OnSwitchLanguage = null;
		}



	}
}