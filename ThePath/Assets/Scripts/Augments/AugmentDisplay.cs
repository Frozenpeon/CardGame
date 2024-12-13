using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI.HUD.Augment
{
    public class AugmentDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _Title;
        [SerializeField] private TextMeshProUGUI _Description;
        [SerializeField] private Image _AugmentSprite;
        [SerializeField] private Image _AugmentIcon;

        [SerializeField] private AugmentSO _AugmentSO;

        public void LoadAugment(AugmentSO pAugment)
        {
            _AugmentSO = pAugment;
            _Title.text = _AugmentSO.augmentName;
            _Description.text = _AugmentSO.augmentDescription;
            _AugmentIcon.sprite = AugmentDisplayDatabase.instance.GetSprite(_AugmentSO);
        }

        public void UpdateSprite(Sprite pNewSprite)
        {
            _AugmentSprite.sprite = pNewSprite;
        }

        public void UpdateDescription(string pNewDescription)
        {
            _Description.text = pNewDescription;
        }

        public void DeactivateIcon()
        {
            _AugmentIcon.enabled = false;
        }

        public void UpdateTitle(string pNewTitle)
        {
            _Title.text = pNewTitle;
        }

        public AugmentSO GetAugmentSO()
        {
            return _AugmentSO;
        }
    }
}
