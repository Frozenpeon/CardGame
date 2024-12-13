using Com.IsartDigital.F2P.Manager.Currency;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Com.IsartDigital.F2P.IAP
{
    public class AddCurrency : MonoBehaviour
    {
        [SerializeField] private CurrencyType _CurrencyType;
        [SerializeField] private int _SoftCurrencyToAdd;
        [SerializeField] private int _HardCurrencyToAdd;
        public UnityEvent onBuy;
        private void Start()
        {

        }
        public void UpdateCurrency()
        {
            CurrencyManager.instance.ShowPopUp(CurrencyType.Free,0);
        }
        public void AddSoftCurrency()
        {
            CurrencyManager.instance.AddSoftCurrency(_SoftCurrencyToAdd);
        }
        public void AddHardCurrency()
        {
            CurrencyManager.instance.AddHardCurrency(_HardCurrencyToAdd);
        }
        public void OnClick() => CurrencyManager.onConfirm += OnBuy;
        public void OnBuy()
        {
            onBuy.Invoke();
            CurrencyManager.onConfirm -= OnBuy;
        }
    }
}