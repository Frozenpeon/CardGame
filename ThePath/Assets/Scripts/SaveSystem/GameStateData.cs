using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    /// <summary>
    /// This is the information of the current game we are playing, saved if the game is turned off.
    /// </summary>
    public class GameStateData : SavingObject
    {
        public static GameStateData ActualGameStateData;

        public string GameStateName;
        //TODO : Add the datas you wanna save, such as the Path state, the hand, .....
        public List<int> pool = new List<int>();
        public List<int> deck = new List<int>();
        public List<int> cemetary = new List<int>();
        public uint nMulligan = 5;

        public List<CardSaveInSlot> cardSaveInSlots = new List<CardSaveInSlot>();
        public List<MonsterSaveInSlot> monsterSaveInSlots = new List<MonsterSaveInSlot>();

        public int nSlotToSpawn = 5;
        public uint normalSlotAlreadySpawn = 0;
        public uint indNewCardSlot = 0;
        public uint totalSlotSpawn = 0;
        public uint nStep = 0;
        public uint nMonsterKilled = 0;
        public uint nMonsterKilledTemp = 0;
        public uint nMonsterAlreadySpawn = 0;
        public int dayPassed = 0;

        public int startWheatValue;
        public int startAttackValue;
        public int healthValue = 3;
        public int attackValue = 1;
        public int wheatValue = 3;


        public List<AugmentSO> activAugments = new List<AugmentSO>();
        public List<AugmentGreat> activAugmentGreat = new List<AugmentGreat>();
        public List<AugmentSOBattleLoot> activAugmentSOBattleLoot = new List<AugmentSOBattleLoot>();
        public List<AugmentSOInvestment> activAugmentSOInvestment = new List<AugmentSOInvestment>();
        public List<AugmentSOExhaust> activAugmentSOExhaust = new List<AugmentSOExhaust>();
        public List<AugmentSOUpPartDay> activAugmentSOUpPartDay = new List<AugmentSOUpPartDay>();
        public List<AugmentDecoy> activAugmentDecoy = new List<AugmentDecoy>();
        public List<AugmentSOEgg> activAugmentSOEgg = new List<AugmentSOEgg>();
        public List<AugmentSOPact> activAugmentPact = new List<AugmentSOPact>();

        public List<DayPart> dayPartInSlot = new List<DayPart>();
        public List<Egg> eggs = new List<Egg>();

        public bool isFirstTimeLaunch = true;

        public GameStateData()
        {
            ActualGameStateData = this;
            AddListeners();
        }


        #region Reseting 
        /// <summary>
        /// Method called to reset the ActualGameStateData, used when the game is ended or restarted, to make sure we don't save a gamestate 
        /// we didn't planned on saving.
        /// </summary>
        public void Reset()
        {
            if (ActualGameStateData != null) 
                ActualGameStateData.RemoveListeners();
            ActualGameStateData = new GameStateData();
        }

        public void AddListeners()
        {
            GameStateChanges.GameEnded += Reset;
            GameStateChanges.GameRestarted += Reset;
        }

        public void RemoveListeners()
        {
            GameStateChanges.GameEnded -= Reset;
            GameStateChanges.GameRestarted -= Reset;
        }
        #endregion
    }
}
