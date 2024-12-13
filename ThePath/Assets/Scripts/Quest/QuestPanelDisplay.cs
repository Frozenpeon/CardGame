using Com.IsartDigital.F2P.IAP;
using Com.IsartDigital.F2P.Manager.Currency;
using Com.IsartDigital.F2P.SO.QuestSO;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    public class QuestPanelDisplay : MonoBehaviour
    {
        [SerializeField] private QuestSO _Quest;
        [SerializeField] private TextMeshProUGUI _QuestDescriptionText;
        [SerializeField] private Image _ProgressBar;
        [SerializeField] private TextMeshProUGUI _ProgressText;
        [SerializeField] private GameObject _QuestRewardActiveButton;
        [SerializeField] private GameObject _QuestRewardDisabledButton;
        private bool _RewardTaken = false;
        private float _Progress = 0f;
        public float ProgressBarDuration = 2f;

        public QuestSO Quest
        {
            get { return _Quest; }
            set
            {
                _Quest = value;
                RegisterQuestProgressUpdate();
                LoadInfos();
            }
        } 

        private void RegisterQuestProgressUpdate()
        {
            if (_Quest != null) _Quest.OnProgressChanged -= UpdateProgressDisplay;

            _Quest.OnProgressChanged += UpdateProgressDisplay;
        }

        private void UpdateProgressDisplay()
        {
            ActivateRewardButton();
            LoadProgress();
        }

        private void OnEnable()
        {
            CheckQuest();
            if (Quest.progress != 0 && (Quest.progress > _Progress))
            {
                _Progress = Quest.progress;
                UpdateProgressDisplay();
                StartCoroutine(UpdateProgressBar());
            }
        }

        private void OnDisable()
        {
            _ProgressBar.fillAmount = (float)_Quest.progress / _Quest.GetGoalAmount();
        }

        private void CheckQuest()
        {
            if (_Quest == null)
            {
                int siblingIndex = transform.GetSiblingIndex();
                Quest = QuestManager.instance.activeQuests[siblingIndex].quest;
            }
        }

        private IEnumerator UpdateProgressBar()
        {
            float targetFillAmount = (float)_Quest.progress / _Quest.GetGoalAmount();
            float initialFillAmount = _ProgressBar.fillAmount;
            float duration = ProgressBarDuration; // Duration in seconds
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _ProgressBar.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsed / duration);
                yield return null;
            }

            _ProgressBar.fillAmount = targetFillAmount;
        }


        public void LoadInfos()
        {
            LoadDescription();
            UpdateProgressDisplay();
        }

        private void LoadDescription()
        {
            _QuestDescriptionText.text = _Quest.description;
        }

        private void LoadProgress()
        {
            string goalText = _Quest.progress + " / " + _Quest.GetGoalAmount();
            _ProgressText.text = goalText;
        }

        private void ActivateRewardButton()
        {
            if (Quest.progress == Quest.GetGoalAmount() )
            {
                QuestManager.QuestCompleted();
                _QuestRewardActiveButton.transform.SetAsLastSibling();
            }
            else
            {
                _QuestRewardDisabledButton.transform.SetAsLastSibling();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                ActivateRewardButton();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                ReloadProgressBar();
            }
        }

        private void ReloadProgressBar()
        {
            StopAllCoroutines();
            _ProgressBar.fillAmount = 0f; // Restart from 0
            StartCoroutine(UpdateProgressBar());
        }

        public void CollectReward()
        {
            _RewardTaken = true;
            _QuestRewardDisabledButton.transform.SetAsLastSibling();
            CurrencyManager.instance.ManageCurrency(CurrencyType.Soft, 25);
            CurrencyManager.InvokeUpdateAllText();
            _QuestRewardDisabledButton.GetComponentInChildren<TextMeshProUGUI>().text = "CLAIMED";
        }
    }
}