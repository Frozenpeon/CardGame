using Com.IsartDigital.F2P.SO.QuestSO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    public class QuestSpecialPanelDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI RewardText;
        [SerializeField] private Button QuestRewardButton;
        public bool _RewardTaken;

        private void Start()
        {
            QuestRewardButton.interactable = false;
        }

        public void ActivateRewardButton()
        {
            QuestRewardButton.interactable = true;
        }

        public void CollectReward()
        {
            _RewardTaken = true;
            QuestRewardButton.interactable = false;
            QuestRewardButton.GetComponentInChildren<TextMeshProUGUI>().text = "CLAIMED";
        }
    }
}
