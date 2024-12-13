using Com.IsartDigital.F2P.Analytics;
using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.Game;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using Com.IsartDigital.F2P.Game.FTUE;
using System.Reflection;
using UnityEngine.Localization.SmartFormat.Core.Output;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier

    /// <summary>
    /// Class that's going to handle the augment system, such as the possible augments, the actives one
    /// the distribution and shuffle of the augment deck. 
    /// </summary>

    public class AugmentHandler : MonoBehaviour, ISavedGameElement
    {
        public AugmentListSO augmentList;
        public int numberOfAugments;
        public int minimumNumberOfAugment;
        private List<AugmentLoader> loaderList;
        [SerializeField] public List<AugmentSO> activAugments;
        [HideInInspector] public List<AugmentSO> augmentAlreadySpawn;
        public List<AugmentSO> FTUEAugments = new List<AugmentSO>();
        [SerializeField] private GameObject prefabAugment;
        [SerializeField] private GameObject image;

        public static event Action AugmentHide;
        public static event Action AugmentShow;

        [SerializeField] private GetInfo _ChooseEffects;

        public static AugmentHandler Instance {  get; private set; }

        private void Start()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);   
            }
           
            Instance = this;
            activAugments = new List<AugmentSO>();
            GameStateChanges.dayPassed += DayEnd;
            GameStateChanges.enemyDefeated += EnemyDefeated;
            GameStateChanges.cardPlayed += OnCardPlayed;
            PathEventManager.saveData += SaveData;

            if (!PlayerData.ActualPlayerData.isFTUE && AugmentSaver.augmentSaverInstance.AcutalList != null)
                augmentList = AugmentSaver.augmentSaverInstance.AcutalList;          

            if (!GameStateData.ActualGameStateData.isFirstTimeLaunch) LoadData();
                Hide();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                Show();
            }
        }

        /// <summary>
        /// Method to Hide the augment handler and the augment offerd.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            image.SetActive(false);
            AugmentHide?.Invoke();
        }

        /// <summary>
        /// Method used to show the augment handler and it's children. This version keeps the number of augment.
        /// </summary>
        public void Show()
        {
            Show(numberOfAugments);
            image.SetActive(true);
            AugmentShow?.Invoke();
        }

        /// <summary>
        ///  Method used to show the augment handler and it's children. This version is used to change the number of augment.
        /// </summary>
        /// <param name="pNum"></param>
        public void Show(int pNum)
        {
            if (pNum <= minimumNumberOfAugment)
                numberOfAugments = minimumNumberOfAugment;
            else
            numberOfAugments = pNum;
            foreach (Transform t in transform)           
                Destroy(t.gameObject);
            CreateChildren(numberOfAugments);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Method that returns actual <see cref="AugmentLoader"/> that are childrens of this object. This also assign
        /// the value of <paramref name="numberOfLoader"/>.
        /// </summary>
        /// <returns></returns>
        private List<AugmentLoader> GetChildren()
        {
            List<AugmentLoader> resList = new List<AugmentLoader>();
            foreach (Transform lTransform in transform) 
            {
                AugmentLoader lLoader = lTransform.GetComponent<AugmentLoader>();
                if (lLoader != null)
                {
                    resList.Add(lLoader);                   
                }
            }
            numberOfAugments = resList.Count;
            return resList;
        }

        /// <summary>
        /// Creates a number of childs, based on the prefab given.
        /// </summary>
        /// <param name="pNumOfChilds"></param>
        private void CreateChildren(int pNumOfChilds)
        {
            if (Instance == null)
                return;
            loaderList = new List<AugmentLoader>();
            GameObject lGO;
            for (int i = 0; i < pNumOfChilds; i++)
            {
                lGO = Instantiate(prefabAugment, Instance.transform);
                loaderList.Add(lGO.GetComponent<AugmentLoader>());
            }
            LoadAugmentList(FTUEManager.instance && FTUEManager.instance.state >= FTUEState.FirstAugment && FTUEManager.instance.state < FTUEState.Menu ? FTUEAugments : GenerateAugmentList());
        }

        /// <summary>
        /// Method that will generate a list of <see cref="AugmentSO"/>. The list length should match the number of
        /// <see cref="AugmentLoader"/> that the method GetChildren return, which is stocked in 
        /// <paramref name="numberOfLoader"/>.
        /// </summary>
        /// <returns></returns>
        private List<AugmentSO> GenerateAugmentList()
        {
            List<AugmentSO> resList = new List<AugmentSO>();
            List<int> ints = new List<int>();
            while (ints.Count < numberOfAugments)
            {
                int randomInt = UnityEngine.Random.Range(0, augmentList.AugmentList.Count);
                if (!ints.Contains(randomInt) && (!(augmentList.AugmentList[randomInt] is AugmentSOEgg) || ((augmentList.AugmentList[randomInt] is AugmentSOEgg) && EggManager.instance.eggs.Find(x => x.partOfDayEating == (augmentList.AugmentList[randomInt] as AugmentSOEgg).partOfDayEating) == null)))
                    ints.Add(randomInt);
            }
            int temp;
            foreach (int i in ints)
            {
                temp = i;
                if (i >= augmentList.AugmentList.Count)
                    temp = i % augmentList.AugmentList.Count;
                resList.Add(augmentList.AugmentList[temp]);
            }
            return resList;
        }

        /// <summary>
        /// Load the augments in the parameter list onto the loaders.
        /// </summary>
        /// <param name="pAugments"></param>
        public void LoadAugmentList(List<AugmentSO> pAugments)
        {
            if (loaderList.Count <= 0)
                return;

            for (int i = 0; i < numberOfAugments; i ++)
            {
              
                loaderList[i].LoadAnAugment(pAugments[i]);
            }
        }


        #region Augment activ list 
        /// <summary>
        /// Method that add a clone of the parameter <see cref="AugmentSO"/> to the <paramref name="activAugments"/> list.
        /// </summary>
        /// <param name="pAugment"></param>
        public void AddActiveAugment(AugmentSO pAugment)
        {
            Hide();
            activAugments.Add((AugmentSO)pAugment.Clone());

            StringBuilder lAugmentsNotChose = new StringBuilder();
            foreach (AugmentLoader lAugment in loaderList)
            {
                lAugmentsNotChose.Append(lAugment.augmentSO.augmentName);
            }
            List<object> lList = new List<object>()
            {
                pAugment.augmentName, PlayerData.ActualPlayerData.PlayerDataID, lAugmentsNotChose.ToString()
            };
            foreach (ParametersData lParameter in _ChooseEffects.eventData.parameters)
            {
                _ChooseEffects.GetValue(lParameter.paramName, lList[_ChooseEffects.eventData.parameters.IndexOf(lParameter)]);
            }
            _ChooseEffects.SendEvent();
        }

        
        public void RemoveActiveAugment(AugmentSO pAugment)
        {
            if (activAugments.Contains(pAugment))
                activAugments.Remove(pAugment);
        }

        public void DayEnd()
        {
            foreach (AugmentSO pAugment in activAugments)
            {
                pAugment.OnDayEnd();
            }
        }

        public void EnemyDefeated()
        {
            foreach (AugmentSO pAugment in activAugments)
            {
                pAugment.OnenemyDefeated();
            }
        }
        public void OnCardPlayed(PlayableCard pCard)
        {
            int lCount = activAugments.Count;
            for (int i = 0; i < lCount; i++)
            {
                AugmentSO pAugment = activAugments[i];
                pAugment.OnCardPlayed(pCard);
            }
        }

        public int GetDamageReduction()
        {
            int value = 0;
            foreach (AugmentSO pAugment in activAugments)
            {
                if (pAugment is AugmentSOExhaust)
                    value += ((AugmentSOExhaust)pAugment).MonsterPowerDowngrade;
            }
            return value;

        }
        #endregion

        public int NewGetDailyUpKeep()
        {
            int lValue = 0;
            List<AugmentSO> lList = activAugments.FindAll(x => x is AugmentSOExhaust);
            foreach (AugmentSO aumgent in lList)
            {
                lValue += (aumgent as AugmentSOExhaust).DailyUpKeppIncrease;
            }
            return lValue;
        }
        public bool IfDecoyAugmentIsActive() => activAugments.Find(x => x is AugmentDecoy);
        public int GetNSlotToNerf() => IfDecoyAugmentIsActive() ? (activAugments.Find(x => x is AugmentDecoy) as AugmentDecoy).nSlotAffected : 0;

        public bool ContainMagicMole(DayPart pDayPart) => activAugments.Find(x => x is AugmentSOEgg && (x as AugmentSOEgg).partOfDayEating == pDayPart);

        public bool ContainDecoy() => activAugments.Find(x => x is AugmentDecoy);

        public void SaveData()
        {
            List<AugmentSO> lActivAugments = new List<AugmentSO>();
            List<AugmentGreat> lActivAugmentGreat = new List<AugmentGreat>();
            List<AugmentSOBattleLoot> lActivAugmentSOBattleLoot = new List<AugmentSOBattleLoot>();
            List<AugmentSOInvestment> lActivAugmentSOInvestment = new List<AugmentSOInvestment>();
            List<AugmentSOExhaust> lActivAugmentSOExhaust = new List<AugmentSOExhaust>();
            List<AugmentSOUpPartDay> lActivAugmentSOUpPartDay = new List<AugmentSOUpPartDay>();
            List<AugmentDecoy> lActivAugmentDecoy = new List<AugmentDecoy>();
            List<AugmentSOEgg> lActivAugmentSOEgg = new List<AugmentSOEgg>();
            List<AugmentSOPact> lActivAugmentPact = new List<AugmentSOPact>();

            foreach (AugmentSO lAugment in activAugments)
            {
                if (lAugment is AugmentGreat)
                {
                    (lAugment as AugmentGreat).UnSubcribToEvents();
                    lActivAugmentGreat.Add(lAugment as AugmentGreat);
                }
                else if (lAugment is AugmentSOBattleLoot)
                {
                    (lAugment as AugmentSOBattleLoot).UnSubcribToEvents();
                    lActivAugmentSOBattleLoot.Add(lAugment as AugmentSOBattleLoot);
                }
                else if (lAugment is AugmentSOInvestment)
                {
                    (lAugment as AugmentSOInvestment).UnSubcribToEvents();
                    lActivAugmentSOInvestment.Add(lAugment as AugmentSOInvestment);
                }
                else if (lAugment is AugmentSOExhaust)
                {
                    (lAugment as AugmentSOExhaust).UnSubcribToEvents();
                    lActivAugmentSOExhaust.Add(lAugment as AugmentSOExhaust);
                }
                else if (lAugment is AugmentSOUpPartDay)
                {
                    (lAugment as AugmentSOUpPartDay).UnSubcribToEvents();
                    lActivAugmentSOUpPartDay.Add(lAugment as AugmentSOUpPartDay);
                }
                else if (lAugment is AugmentDecoy)
                {
                    (lAugment as AugmentDecoy).UnSubcribToEvents();
                    lActivAugmentDecoy.Add(lAugment as AugmentDecoy);
                }
                else if (lAugment is AugmentSOEgg)
                {
                    (lAugment as AugmentSOEgg).UnSubcribToEvents();
                    lActivAugmentSOEgg.Add(lAugment as AugmentSOEgg);
                }
                else if (lAugment is AugmentSOPact)
                {
                    (lAugment as AugmentSOPact).UnSubcribToEvents();
                    lActivAugmentPact.Add(lAugment as AugmentSOPact);
                }
                else
                {
                    lAugment.UnSubcribToEvents();
                    lActivAugments.Add(lAugment);
                }
            }

            GameStateData.ActualGameStateData.activAugments = lActivAugments;
            GameStateData.ActualGameStateData.activAugmentGreat = lActivAugmentGreat;
            GameStateData.ActualGameStateData.activAugmentSOBattleLoot = lActivAugmentSOBattleLoot;
            GameStateData.ActualGameStateData.activAugmentSOInvestment = lActivAugmentSOInvestment;
            GameStateData.ActualGameStateData.activAugmentSOExhaust = lActivAugmentSOExhaust;
            GameStateData.ActualGameStateData.activAugmentSOUpPartDay = lActivAugmentSOUpPartDay;
            GameStateData.ActualGameStateData.activAugmentDecoy = lActivAugmentDecoy;
            GameStateData.ActualGameStateData.activAugmentSOEgg = lActivAugmentSOEgg;
            GameStateData.ActualGameStateData.activAugmentPact = lActivAugmentPact;
        }

        public void LoadData()
        {
            LoadAugmentList<AugmentSO>(GameStateData.ActualGameStateData.activAugments);
            LoadAugmentList<AugmentGreat>(GameStateData.ActualGameStateData.activAugmentGreat);
            LoadAugmentList<AugmentSOBattleLoot>(GameStateData.ActualGameStateData.activAugmentSOBattleLoot);
            LoadAugmentList<AugmentSOInvestment>(GameStateData.ActualGameStateData.activAugmentSOInvestment);
            LoadAugmentList<AugmentSOExhaust>(GameStateData.ActualGameStateData.activAugmentSOExhaust);
            LoadAugmentList<AugmentSOUpPartDay>(GameStateData.ActualGameStateData.activAugmentSOUpPartDay);
            LoadAugmentList<AugmentDecoy>(GameStateData.ActualGameStateData.activAugmentDecoy);
            LoadAugmentList<AugmentSOEgg>(GameStateData.ActualGameStateData.activAugmentSOEgg);
            LoadAugmentList<AugmentSOPact>(GameStateData.ActualGameStateData.activAugmentPact);
        }

        private void LoadAugmentList<T>(List<T> pList) where T : AugmentSO
        {
            for (int i = 0; i < pList.Count; i++)
            {
                activAugments.Add(pList[i]);
                pList[i].SubcribToEvents();
            }                
        } 

        private void OnDestroy()
        {
            GameStateChanges.cardPlayed -= OnCardPlayed;
            PathEventManager.saveData -= SaveData;
            if (Instance == this)
                Instance = null;
        }

    }
}
