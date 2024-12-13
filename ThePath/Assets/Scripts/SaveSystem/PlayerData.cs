using Com.IsartDigital.F2P.Game.FTUE;
using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P  {
    /// <summary>
    /// The global datas we want to save.
    /// </summary>
    public class PlayerData : SavingObject {

        public static PlayerData ActualPlayerData;

        //TODO : Add the exact datas we want to save, such as player ID, deck,....
        public DateTime currentDate;
        public TimeSpan totalTimePlayed;
        public uint nConnectionToday = 0;
        public int PlayerDataID = 0;
        public uint bestStep = 0;
        public string PlayerUsernName = "Name";

        public bool isFTUE = true;
        public FTUEState ftueState = FTUEState.Start;

        public int currentCharetteSkinEquiped = 0;
        public int currentBGSkinEquiped = 0;

        public List<int> charetteSkinPossessed = null;
        public List<int> bgSkinPossessed = null;

        public PlayerData()
        {
            ActualPlayerData = this;
        }

    }
}
