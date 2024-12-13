using Com.IsartDigital.F2P.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Manager.Currency
{
    public class CurrencyManager : MonoBehaviour
    {
        public int gold = 0;
        public int diamond = 0;

        [SerializeField] private GameObject _PopUpBuyItem;

        private ShopData _ShopData => ShopData.ActualShopData;

        public static event Action<CurrencyType, int> updateCurrency;
        public static event Action updateInventory;
        public static event Action updateAllText;
        public static event Action onConfirm;

        #region SINGLETON

        private static CurrencyManager Instance;
        public static CurrencyManager instance
        {
            get => Instance;
            private set => Instance = value;
        }

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
            UIManager.loadData += LoadData;
            UIManager.saveData += SaveData;
            LoadData();
            updateCurrency += ManageCurrency;
        }

        public static void InvokeUpdateInventory() => updateInventory?.Invoke();
        public static void InvokeUpdateAllText() 
        {
            UIManager.InvokeSaveData();
            updateAllText?.Invoke();
        }
        public static void InvokeOnConfirm() => onConfirm.Invoke();
        public static void InvokeUpdateCurrency(CurrencyType pCurrencyType, int pValueToRemove) => updateCurrency?.Invoke(pCurrencyType, pValueToRemove);

        public void ManageCurrency(CurrencyType pCurrencyType, int pValueToRemove)
        {
            switch (pCurrencyType)
            {
                case CurrencyType.Soft:
                    AddSoftCurrency(-pValueToRemove);
                    break;
                case CurrencyType.Hard:
                    AddHardCurrency(-pValueToRemove);
                    break;
                default:
                    break;
            }
            InvokeUpdateInventory();
        }

        public void AddSoftCurrency(int pValueToAdd)
        {
            gold += pValueToAdd;
            InvokeUpdateAllText();
        }
        public void AddHardCurrency(int pValueToAdd)
        {
            diamond  += pValueToAdd;
            InvokeUpdateAllText();
        }

        public void BuyItem(CurrencyType pCurrencyType, int pValueToRemove)
        {
            if (pCurrencyType == CurrencyType.Soft && gold < pValueToRemove)
            {
                return;
            }
            else if (pCurrencyType == CurrencyType.Hard && diamond < pValueToRemove)
            {
                return;
            }

            _PopUpBuyItem.SetActive(true);
            _PopUpBuyItem.GetComponent<CurrencyPopUp>().currencyType = pCurrencyType;
            _PopUpBuyItem.GetComponent<CurrencyPopUp>().valueToRemove = pValueToRemove;
        }

        public void ShowPopUp(CurrencyType pType, int pValue)
        {
            _PopUpBuyItem.SetActive(true);
            _PopUpBuyItem.GetComponent<CurrencyPopUp>().currencyType = pType;
            _PopUpBuyItem.GetComponent<CurrencyPopUp>().valueToRemove = pValue;
        }

        private void LoadData()
        {
            gold = _ShopData.gold;
            diamond = _ShopData.diamond;
            InvokeUpdateAllText();
        }
        private void SaveData()
        {
            _ShopData.gold = gold;
            _ShopData.diamond = diamond;
        }

        private void OnDestroy()
        {
            UIManager.loadData -= LoadData;
            UIManager.saveData -= SaveData;

            updateCurrency -= ManageCurrency;
        }
    }
}