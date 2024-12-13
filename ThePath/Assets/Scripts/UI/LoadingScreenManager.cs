using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.LoadScene
{
    public class LoadingScreenManager : MonoBehaviour
    {
        [SerializeField] private GameObject _LoadingScreen;
        [SerializeField] private Image _Fill;

        private AsyncOperation _AsyncOperation;
        private Coroutine _Coroutine = null;
        #region Singleton
        private static LoadingScreenManager instance;
        public static LoadingScreenManager Instance { get => instance; private set => instance = value; }
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion
        void Start()
        {
            _LoadingScreen?.SetActive(false);
        }

        void Update()
        {

        }

        public void LoadScene(SceneEnum pScene)
        {
            if (_Coroutine == null)
                _Coroutine = StartCoroutine(LoadSceneAsync(pScene));
            else
                Debug.LogError("A Loading screen is currently active");
        }
        private IEnumerator LoadSceneAsync(SceneEnum pScene)
        {
            if (!_Fill) Debug.LogError($"The variable { nameof(_Fill)} is empty");

            _LoadingScreen?.SetActive(true);
            _AsyncOperation = SceneManager.LoadSceneAsync((int)pScene);
            _Fill.fillAmount = 0;
            while (!_AsyncOperation.isDone)
            {
                _Fill.fillAmount = Mathf.Clamp01(_AsyncOperation.progress / 0.9f);
                yield return null;
            }
            _LoadingScreen?.SetActive(false);
            StopCoroutine(_Coroutine);
            _Coroutine = null;
        }
        private void OnDestroy()
        {
            if (instance != null) instance = null;
        }
    }
}