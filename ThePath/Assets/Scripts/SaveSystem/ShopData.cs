using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    /// <summary>
    /// The datas about the shop.
    /// </summary>
    public class ShopData : SavingObject
    {
        public static ShopData ActualShopData;

        public int gold = 2000;
        public int diamond = 2000;

        public ShopData()
        {
            ActualShopData = this;
        }
    }
}
