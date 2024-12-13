using Com.IsartDigital.F2P.Manager.Currency;
using TMPro;
using UnityEngine;

namespace Com.IsartDigital.F2P.IAP
{
    public class CurrencyBtn : MonoBehaviour
    {
        [SerializeField] private CurrencyType _Currency;
        [SerializeField] private TextMeshProUGUI _Text;

        void Start()
        {
            CurrencyManager.updateAllText += UpdateText;
            CurrencyManager.InvokeUpdateAllText();
        }

        private void UpdateText()
        {
            _Text.text = (_Currency == CurrencyType.Soft ? CurrencyManager.instance.gold : CurrencyManager.instance.diamond).ToString();
        }
        private void OnDestroy()
        {
            CurrencyManager.updateAllText -= UpdateText;
        }
    }
}
