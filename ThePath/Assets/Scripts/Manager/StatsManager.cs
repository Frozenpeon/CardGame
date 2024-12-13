using Com.IsartDigital.F2P.Game;
using Com.IsartDigital.F2P.Manager;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    public class StatsManager : MonoBehaviour, ISavedGameElement
    {
        private int healthValue;
        private int maxHealthValue;
        [SerializeField] private int attackValue;
        [SerializeField] private int wheatValue;

        int lTempMaxHealthValue;
        int lTempHealthValue;
        int lTempAttackValue;
        int lTempWheatValue;

        public int startWheatValue;
        public int startAttackValue;


        [Space(10), Header("Texts objects")]
        [SerializeField] private TextMeshProUGUI healthValueText;
        [SerializeField] private TextMeshProUGUI attackValueText;
        [SerializeField] private TextMeshProUGUI wheatValueText;

        [Space(10), Header("Colors")]
        [SerializeField] private Color _ColorValueDecreasing;
        [SerializeField] private Color _ColorValueIncreasing;
        [SerializeField] private Color _ColorValueDefault;

        private GameManager _GameManager => GameManager.instance;
        static public StatsManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            EventManager.OnCardPlayed += ApplyCardEffects;
            GameStateChanges.ressourceChange += UpdateHUDByAugment;
            GameStateChanges.dayPassed += OnDayPassed;
        }

        private void Start()
        {
            PathEventManager.saveData += SaveData;
            PathEventManager.loadData += LoadData;
        }

        public string GetAttackValueTxt() => attackValueText.text;
        public string GetWheatValueTxt() => wheatValueText.text;
        public void ApplyCardEffects(CardEffect effect)
        {
            if (effect.maxHealthChange != 0)
            {
                MaxHealthValue += effect.maxHealthChange;
            }
            if (effect.healthChange != 0)
            {
                HealthValue += effect.healthChange;
            }
            if (effect.attackChange != 0)
            {
                AttackValue += effect.attackChange;
            }
            if (effect.wheatChange != 0)
            {
                WheatValue += effect.wheatChange;
            }
            UpdateHUD();
        }

        public void ResetHUD()
        {
            WheatValue = 0;
            AttackValue = 0;
        }

        public int MaxHealthValue
        {
            get { return maxHealthValue; }
            set
            {
                int oldValue = maxHealthValue;
                maxHealthValue = value;

                int difference = maxHealthValue - oldValue;
                HealthValue += difference;

                if (healthValueText != null)
                {
                    maxHealthValue = Mathf.Clamp(maxHealthValue, 0, int.MaxValue);
                    healthValueText.text = healthValue.ToString() + "/" + maxHealthValue.ToString();
                }
            }
        }

        public int HealthValue
        {
            get { return healthValue; }
            set
            {
                healthValue = value;
                if (healthValueText != null)
                {
                    healthValue = Mathf.Clamp(healthValue, 0, maxHealthValue);
                    healthValueText.text = healthValue.ToString() + "/" + maxHealthValue.ToString();
                }
                if (healthValue == 0)
                {
                    EventManager.TriggerPlayOccured(SO.QuestSO.PlayType.Game);
                    _GameManager.GameOver();
                }
            }
        }

        public int AttackValue
        {
            get { return attackValue; }
            set
            {
                attackValue = value;
                if (attackValueText != null)
                    attackValue = Mathf.Clamp(attackValue, 0, int.MaxValue);
                    attackValueText.text = attackValue.ToString();
            }
        }

        public int WheatValue
        {
            get { return wheatValue; }
            set
            {
                wheatValue = value;
                if (wheatValueText != null)
                    wheatValue = Mathf.Clamp(wheatValue, 0, int.MaxValue);  
                    wheatValueText.text = wheatValue.ToString();
            }
        }

        private void OnDestroy()
        {
            PathEventManager.saveData -= SaveData;
            PathEventManager.loadData -= LoadData;
            EventManager.OnCardPlayed -= ApplyCardEffects;
            GameStateChanges.ressourceChange -= UpdateHUDByAugment;
            GameStateChanges.dayPassed -= OnDayPassed;
        }

        public void StoreTempValues()
        {
            lTempMaxHealthValue = MaxHealthValue;
            lTempHealthValue = HealthValue;
            lTempAttackValue = AttackValue;
            lTempWheatValue = WheatValue;
        }

        public void UpdateHUD()
        {
            UpdateTextColor(healthValueText, lTempMaxHealthValue, MaxHealthValue);
            UpdateTextColor(healthValueText, lTempHealthValue, HealthValue);
            UpdateTextColor(attackValueText, lTempAttackValue, AttackValue);
            UpdateTextColor(wheatValueText, lTempWheatValue, WheatValue);
        }

        private void UpdateHUDByAugment(Ressources pRessources, int pValue, DayPart pPartDay)
        {
            CardEffect lEffect = new CardEffect(maxHealth: pRessources == Ressources.maxLife ? pValue : 0, 
                                                health: pRessources == Ressources.life ? pValue : 0, 
                                                damage: pRessources == Ressources.attack ? pValue : 0, 
                                                wheat: pRessources == Ressources.wheat ? pValue : 0);

            EventManager.CardPlayed(lEffect);
            startAttackValue += pRessources == Ressources.attack ? pValue : 0;
            startWheatValue += pRessources == Ressources.wheat ? pValue : 0;
            UpdateHUD();
        }

        private void UpdateTextColor(TextMeshProUGUI textComponent, int oldValue, int newValue)
        {
            if (textComponent != null)
            {
                textComponent.color = GetColorByValueChange(oldValue, newValue);
            }
        }

        private Color GetColorByValueChange(int oldValue, int newValue)
        {
            if (newValue > oldValue)
            {
                return _ColorValueIncreasing;
            }
            else if (newValue < oldValue)
            {
                return _ColorValueDecreasing;
            }
            else
            {
                return _ColorValueDefault;
            }
        }

        private void OnDayPassed()
        {
            startWheatValue -= 1;
            WheatValue -= 1;
        }

        public void SaveData()
        {
            GameStateData.ActualGameStateData.startAttackValue = startAttackValue;
            GameStateData.ActualGameStateData.startWheatValue = startWheatValue;
            GameStateData.ActualGameStateData.wheatValue = WheatValue;
            GameStateData.ActualGameStateData.attackValue = AttackValue;
            GameStateData.ActualGameStateData.healthValue = HealthValue;
        }
        public void LoadData()
        {
            startAttackValue = GameStateData.ActualGameStateData.startAttackValue;
            startWheatValue = GameStateData.ActualGameStateData.startWheatValue;
            WheatValue = startWheatValue = GameStateData.ActualGameStateData.wheatValue;
            AttackValue = startAttackValue = GameStateData.ActualGameStateData.attackValue;
            HealthValue = GameStateData.ActualGameStateData.healthValue;

            StoreTempValues();
            UpdateHUD();
        }     

    }
}
