using Com.IsartDigital.F2P.Game.FTUE;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public static class PathEventManager
    {
        public static event Action<DayPart, int> updateEgg;

        public static bool isFirstTimeLaunch = true;
        public static event Action saveData;
        public static event Action loadData;
        public static event Action<int> updateMulligan;
        public static event Action<int> stepChanged;
        public static event Action onLifeLost;
        public static event Action onCardPlayed;
        public static event Action onCardUpdated;
        public static event Action onPathMoved;
        public static event Action<float> monsterKilledRatio;
        public static event Action<int> monsterKilled;
        public static event Action<int> onMonsterKilled;

        public static event Action<int> dayPassed;


        public static event Action onGainedArrow;
        public static event Action onGainedWheat;
        public static event Action onBossKilled;
        public static event Action onDailyUpKeep;
        public static event Action onCardDraw;
        public static event Action onEndOfTurnBtn;
        public static event Action onHUDBtn;
        public static event Action onBtnMulligan;
        public static event Action playKilledMonster;


        public static event Action onAugmentChoose;
        public static event Action onMulligan;
        public static event Action desactivateHand;
        public static event Action moveHand;



        public static event Action<StepEvent, FTUEState> onFTUEStepChanged;
        /// <summary>
        /// Event was Invoke when the player changed step
        /// </summary>
        /// <param name="pStep"></param>
        public static void InvokeStepChanged(int pStep) => stepChanged?.Invoke(pStep);
        
        public static void InvokeMonsterKilledRatio(float pNbMonsterKilledRatio) => monsterKilledRatio?.Invoke(pNbMonsterKilledRatio);
        public static void InvokeMonsterKilled(int pNMonsterKilled) => monsterKilled?.Invoke(pNMonsterKilled);
        public static void InvokeOnMonsterKilled(int pNbMonsterKilled) => onMonsterKilled?.Invoke(pNbMonsterKilled);
        public static void InvokeDayPassed(int pNDayPassed) => dayPassed?.Invoke(pNDayPassed);

        public static void InvokeUpdateMulligan(int pValueToAdd) => updateMulligan.Invoke(pValueToAdd);
        public static void InvokeOnLifeLost() => onLifeLost.Invoke();
        public static void InvokeSaveData() => saveData.Invoke();
        public static void InvokeLoadData() => loadData.Invoke();

        public static void InvokeOnCardPlayed() => onCardPlayed?.Invoke();
        public static void InvokeOnCardUpdated() => onCardUpdated?.Invoke();
        public static void InvokeUpdateEgg(DayPart pDayPart, int pValueToAdd) => updateEgg?.Invoke(pDayPart, pValueToAdd);



        public static void InvokeOnBtnMulligan() => onBtnMulligan?.Invoke();
        public static void InvokeOnGainedArrow() => onGainedArrow?.Invoke();
        public static void InvokeOnGainedWheat() => onGainedWheat?.Invoke();
        public static void InvokeOnBossKilled() => onBossKilled?.Invoke();
        public static void InvokeOnDailyUpKeep() => onDailyUpKeep?.Invoke();
        public static void InvokeOnCardDraw() => onCardDraw?.Invoke();
        public static void InvokeOnEndOfTurnBtn() => onEndOfTurnBtn?.Invoke();
        public static void InvokeOnHUDBtn() => onHUDBtn?.Invoke();
        public static void InvokePlayKilledMonster() 
        {
            playKilledMonster?.Invoke();
        } 
        public static void InvokeOnPathMoved() => onPathMoved?.Invoke();

        public static void InvokeOnFTUEStepChanged(StepEvent pEventConfig, FTUEState pFTUEState) => onFTUEStepChanged.Invoke(pEventConfig, pFTUEState);
    
        public static void InvokeOnAugmentChoose() => onAugmentChoose?.Invoke();
        public static void InvokeOnMulligan() => onMulligan?.Invoke();
        public static void InvokeDesactivateHand() => desactivateHand?.Invoke();
        public static void InvokeMoveHand() => moveHand?.Invoke();
    }
}
