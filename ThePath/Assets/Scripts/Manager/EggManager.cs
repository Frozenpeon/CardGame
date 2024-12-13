using Com.IsartDigital.F2P.Game;
using Com.IsartDigital.F2P.UI.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Com.IsartDigital.F2P.Manager
{

    public enum EggType
    {
        Mole, Dragon
    }
    [Serializable] public class Egg
    {
        public Ressources ressourceNeeded;
        public int amountRessourceNeeded;
        public Ressources ressourceRewarded;
        public int amountRessourceRewarded;
        public int currentProgress;
        public DayPart partOfDayEating;

        public Egg(Ressources pResNeed, int pResNeedAmount, Ressources pResGiven, int pResGivenAmount, DayPart pDayPart)
        {
            ressourceNeeded = pResNeed;
            amountRessourceNeeded = pResNeedAmount;
            ressourceRewarded = pResGiven;
            amountRessourceRewarded = pResGivenAmount;
            partOfDayEating = pDayPart;

            PathEventManager.updateEgg += UpdateEggProgress;
        }
        public void UpdateEggProgress(DayPart pDayPart, int pValueToAdd)
        {
            currentProgress += pDayPart == partOfDayEating ? pValueToAdd : 0;
            if (currentProgress >= amountRessourceNeeded)
            {
                StatsManager lStatsManager = StatsManager.instance;
                lStatsManager.startWheatValue += ressourceRewarded == Ressources.wheat ? amountRessourceRewarded : 0;
                lStatsManager.startAttackValue += ressourceRewarded == Ressources.attack ? amountRessourceRewarded : 0;
                
                lStatsManager.UpdateHUD();
                EggManager.InvokeOnEggFinish(ressourceNeeded == Ressources.wheat ? EggType.Dragon : EggType.Mole);
                DestroyEgg();
            }
        }
        public void UnscibeToEvent()
        {
            PathEventManager.updateEgg -= UpdateEggProgress;
        }

        public void DestroyEgg()
        {
            UnscibeToEvent();
        }
    }
    public class EggManager : MonoBehaviour, ISavedGameElement
    {

        public static event Action<EggType> onNewEgg;
        public static event Action<EggType> onEggFinish;
        public List<Egg> eggs = new();
        #region Singleton
        private static EggManager Instance;
        public static EggManager instance
        {
            get => Instance;
            private set => Instance = value;
        }

        private void Awake()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
        }
        #endregion

        private void Start()
        {
            PathEventManager.saveData += SaveData;
            PathEventManager.loadData += LoadData;
        }
        public void LoadData()
        {
            eggs = new List<Egg>(GameStateData.ActualGameStateData.eggs);
        }
        public void SaveData()
        {
            GameStateData.ActualGameStateData.eggs = new List<Egg>(eggs);
        }

        public static void InvokeOnNewEgg(EggType pType) => onNewEgg?.Invoke(pType);
        public static void InvokeOnEggFinish(EggType pType) => onEggFinish?.Invoke(pType);

        private void OnDestroy()
        {
            PathEventManager.saveData -= SaveData;
            PathEventManager.loadData -= LoadData;
        }
    }
}