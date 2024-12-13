using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P {
    // Author : Thomas Verdier


    /// <summary>
    /// The grouped data of what we need to save. 
    /// </summary>
    public class SavedDatas {

        public PlayerData playerData;
        public GameStateData gameStateData;
        public ShopData shopData;
        public SettingsData settingsData;
        public AugmentSaver augmentSaver;

        public SavedDatas(AugmentListSO pList) { 
            playerData = new PlayerData();
            gameStateData = new GameStateData();
            shopData = new ShopData();
            settingsData = new SettingsData();
            augmentSaver = new AugmentSaver(pList);
            Loading();
        }

        /// <summary>
        /// Method that gives the statics instance of each parameters the values of the acual <see cref="SavedDatas"/>.
        /// </summary>
        public void Loading()
        {
            PlayerData.ActualPlayerData = playerData;
            GameStateData.ActualGameStateData = gameStateData;
            ShopData.ActualShopData = shopData;
            SettingsData.ActualSettingsData = settingsData;
            AugmentSaver.augmentSaverInstance = augmentSaver;
        }


    }


}
