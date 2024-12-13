using Com.IsartDigital.F2P.Manager.Currency;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.IAP
{
    public enum CurrencyType
    {
        Free, Soft, Hard
    }
    public class ShopItemBtn : MonoBehaviour
    {
        [SerializeField] private Button _Button;
        [SerializeField] private CurrencyType _CurrencyType;
        [SerializeField] private int _CurrencyValueToRemove;

        public UnityEvent onBuy;

        private void Start()
        {

        }
        public void UpdateCurrency()
        {
            CurrencyManager.instance.BuyItem(_CurrencyType, _CurrencyValueToRemove);
        }
        public void OnClick() => CurrencyManager.onConfirm += OnBuy;
        public void OnBuy()
        {
            onBuy.Invoke();
            CurrencyManager.onConfirm -= OnBuy;
        }

        private void OnDestroy()
        {
        }
    }
}