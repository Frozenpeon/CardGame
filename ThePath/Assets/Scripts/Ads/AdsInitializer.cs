using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Com.IsartDigital.F2P.Ads
{
    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField] string _androidGameId;
        [SerializeField] string _iOSGameId;
        [SerializeField] bool _testMode = true;
        private string _gameId;

        public List<RewardedAdsButton> rewardedAds;

        private static AdsInitializer Instance;
        public static AdsInitializer instance
        {
            get => Instance;
            private set => Instance = value;
        }

        void Awake()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
        }
        private void Start()
        {
            InitializeAds();
        }

        public void InitializeAds()
        {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
            _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
        }


        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");

            foreach (RewardedAdsButton lAd in rewardedAds)
            {
                lAd.LoadAd();
            }
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
        }
    }
}