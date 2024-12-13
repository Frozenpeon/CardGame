using Com.IsartDigital.F2P.Manager.Currency;
using System;
using UnityEngine;

namespace Com.IsartDigital.F2P.IAP
{
    public class CurrencyPopUp : MonoBehaviour
    {
        [HideInInspector] public CurrencyType currencyType;
        [HideInInspector] public int valueToRemove;

        public void ConfirmPurchase()
        {
            CurrencyManager.InvokeOnConfirm();
            CurrencyManager.instance.ManageCurrency(currencyType, valueToRemove);
            CurrencyManager.InvokeUpdateAllText();
        }

        public void ClosePopUp()
        {
            gameObject.SetActive(false);
        }

    }
}
