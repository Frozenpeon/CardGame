using Com.IsartDigital.F2P.Game;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Cards
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private GameObject _DailyUpKeep;
        [SerializeField] private MonsterCard _MonsterCardPrefab;
        public Button btn;
        [SerializeField] private Image _SlotImage;

        [SerializeField] private TextMeshProUGUI _DayPartTextIndicator;
        [SerializeField] private TextMeshProUGUI _DailyUpKeepText;
        [SerializeField] private List<Sprite> _DaypartImage = new();

        [NonSerialized] public Vector3 nextPose;
        [NonSerialized] public Vector3 startPose;
        private string _DailyUpKeepIcone;
        public bool isUsed => CheckIfCardIsUsed();

        [NonSerialized] public GameObject card;
        public Monster monster = null;
        public bool monsterIsBoss = false;
        public Ressources resGained => GetRessourceInPath();

       [SerializeField] private LayerMask _MonsterSlotLayer;
        private RectTransform _RectTransform => GetComponent<RectTransform>();


        public DayPart dayPart = 0;

        public int boostValue = 0;
        public int monsterNerfValue = 0;
        public Ressources ressourceToNerf;

        private void Start()
        {
            _DailyUpKeepIcone = IconsManager.WHEAT;
        }

        private List<List<string>> _Prefix = new List<List<string>>()
        {
            new List<string>() { "Coward", "nKid","Small","Weak" },
            new List<string>() { "Spooky", "Young", "Insolent", "Insolent" },
            new List<string>() { "Scary", "Strong", "Tough", "Bold" },
            new List<string>() { "Spooky", "Young", "Insolent", "Insolent" },
            new List<string>() { "Big", "Brutal", "Powerfull", "Vicious" },
            new List<string>() { "Terrifying", "Horrific", "King", "Huge", "Infernal", "Demoniac", "Colossal", "Deadly" }
        };

        private Ressources GetRessourceInPath()
        {
            Ressources lRes = Ressources.maxLife;

            PlayableCard lCard = GetComponentInChildren<PlayableCard>();

            if (lCard != null)
            {
                if (lCard.AttackValue > 0) lRes = Ressources.attack;
                else if (lCard.WheatValue > 0) lRes = Ressources.wheat;
            }

            return lRes;
        }

        private bool CheckIfCardIsUsed()
        {
            int lCount = transform.childCount;
            for (int i = 0; i < lCount; i++)
            {
                if (transform.GetChild(i).GetComponent<CardDisplay>()) 
                    return true;
            }
            return false;
        }
        public void UpdateDailyUpKeep(int pDailyUpKeep)
        {
            int i = pDailyUpKeep + AugmentHandler.Instance.NewGetDailyUpKeep();
            _DailyUpKeepText.text = "-" + i + " " + IconsManager.WHEAT;
        }
        /// <summary>
        /// Used for test
        /// </summary>
        /// <param name="isMonster"></param>
        public void UpdateSlot(bool pIsMonster, int pDailyUpKeep) 
        {
            UpdateDailyUpKeep(pDailyUpKeep);
            btn.interactable = !pIsMonster;
            gameObject.layer = pIsMonster ? 0 : gameObject.layer;
            if (pIsMonster) LoadMonsterCard();

            monster = null;
        }
        private void LoadMonsterCard()
        {
            gameObject.layer = (int)Mathf.Log(_MonsterSlotLayer, 2);

            MonsterCard lMonsterCard = Instantiate(_MonsterCardPrefab, transform);
            lMonsterCard.GetComponent<RectTransform>().position = _RectTransform.position;
            if (monster != null)
            {
                int lLevel = 0;
                if (monster.monsterSO)
                {
                    lLevel = Mathf.Clamp(monster.monsterSO.level, 0, _Prefix.Count);
                    lMonsterCard.title.text = _Prefix[lLevel][UnityEngine.Random.Range(0, _Prefix[lLevel].Count)] + " " + monster.monsterSO.cardName;
                    monsterIsBoss = monster.monsterSO.isBoss;
                    if (monsterIsBoss)
                    {
                        lMonsterCard.isBoss = true;
                        lMonsterCard.BossArrowValue.text = monster.monsterSO.attackRemoved.ToString();
                        lMonsterCard.BossWheatValue.text = monster.monsterSO.wheatRemoved.ToString();
                    }
                    else
                    {
                        lMonsterCard.isBoss = false;
                        if (monster.monsterSO.attackRemoved != 0)
                        {
                            lMonsterCard.monsterType = MonsterCard.MonsterType.gobelin;
                            lMonsterCard.MobArrowValue.text = monster.monsterSO.attackRemoved.ToString();
                        }
                        else if (monster.monsterSO.wheatRemoved != 0)
                        {
                            lMonsterCard.monsterType = MonsterCard.MonsterType.bandit;
                            lMonsterCard.MobWheatValue.text = monster.monsterSO.wheatRemoved.ToString();
                        }
                    }
                }
                lMonsterCard.LoadInfo();
                dayPart = DayPart.Default;
                _DailyUpKeep.SetActive(false);
            }
        }

        public void SetDayPartImage()
        {
            if (dayPart != DayPart.Default) _SlotImage.sprite = _DaypartImage[(int)dayPart];
            _DayPartTextIndicator.text = dayPart.ToString();

            _DailyUpKeep.SetActive(dayPart == DayPart.Night);
        }
    }
}
