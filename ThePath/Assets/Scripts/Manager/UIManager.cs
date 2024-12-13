using Com.IsartDigital.F2P.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Com.IsartDigital.F2P.Manager
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField] private List<GameObject> _screenList = new List<GameObject>();
        [SerializeField] private List<GameObject> _overlayList = new List<GameObject>();
		[SerializeField] private TypeOfScreen _currentScreen = TypeOfScreen.None;
		[SerializeField] private TypeOfOverlay _currentOverlay = TypeOfOverlay.None;

		[SerializeField] private GameObject _BG;

		private bool _isOverlayed;

		public static event Action saveData;
		public static event Action loadData;

		public enum TypeOfScreen
		{
			MainMenu,
			None
		}

		public enum TypeOfOverlay
		{
			Settings,
			Quest,
			PlayerProfil,
			None
		}

		private static UIManager instance;

		private UIManager() { }

		public static UIManager GetInstance
		{
			get { return instance; }
			private set { instance = value; }
		}

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				Debug.Log("This instance of " + GetType().Name +" already exist, destroying the last one added");
				return;
			}
			else instance = this;

			DontDestroyOnLoad(gameObject);

			SettingsManager.SettingsInit();
		}

		void Start()
		{
			InvokeLoadData();

            SwitchToMainMenu();
		}

		/// <summary>
		/// Disable current screen gameObject to replace it by parameters screen.
		/// </summary>
		/// <param name="pScreen">Screen to enable</param>
		private void SwitchScreen(TypeOfScreen pScreen)
		{
			_screenList[(int)_currentScreen].SetActive(false);
			_currentScreen = pScreen;
			_screenList[(int)_currentScreen].SetActive(true);
		}

		/// <summary>
		/// Enable Main Menu gameObject
		/// </summary>
		public void SwitchToMainMenu()
		{
			SwitchScreen(TypeOfScreen.MainMenu);
		}

		public void UnShowAll()
		{
            SwitchScreen(TypeOfScreen.None);
            SwitchOverlay(TypeOfOverlay.None);
            SwitchOverlay(TypeOfOverlay.None);
        }

		public void ShowBG(bool pShowBG)
        {
            _BG.SetActive(pShowBG);
        }

		/// <summary>
		/// Depends on the state, disable or enable overlay to display on screen
		/// </summary>
		/// <param name="pOverlay">QuestOverlay to enable</param>
		private void SwitchOverlay(TypeOfOverlay pOverlay)
		{
			if (!_isOverlayed)
			{
				//_screenList[(int)_currentScreen].SetActive(false);
				_currentOverlay = pOverlay;
				_overlayList[(int)_currentOverlay].SetActive(true);
				_isOverlayed = true;
			}
			else
			{
				_overlayList[(int)_currentOverlay].SetActive(false);
				_currentOverlay = TypeOfOverlay.None;
				_isOverlayed = false;
				//_screenList[(int)_currentScreen].SetActive(true);
			}
		}

		/// <summary>
		/// Enable or disable Settings overlay
		/// </summary>
		public void SwitchSettingsOverlay()
		{
			SwitchOverlay(TypeOfOverlay.Settings);
		}

        public void SwitchQuestOverlay()
        {
            SwitchOverlay(TypeOfOverlay.Quest);
        }
        public void SwitchPlayerProfilOverlay()
        {
            SwitchOverlay(TypeOfOverlay.PlayerProfil);
        }

        public static void InvokeSaveData() => saveData?.Invoke();
        public static void InvokeLoadData() => loadData?.Invoke();


        private void OnApplicationQuit()
        {
			InvokeSaveData();
            SaveSystem.SaveActualDatas();
        }
    }
}