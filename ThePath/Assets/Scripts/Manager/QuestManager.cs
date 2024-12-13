using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.SO.QuestSO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private const string questDirectory = "ScriptableObject/Quest/Quests";
    [SerializeField] private int _DailyQuestsAmount = 3;
    [SerializeField] private List<int> QuestRewards = new();
    [SerializeField] public List<QuestAndReward> activeQuests = new();
    [SerializeField] private int AllQuestCompletedReward;
    [SerializeField] private GameObject QuestWindow;
    [SerializeField] private GameObject QuestPanelsContainer;

    public int dailyQuestsCompleted = 0;

    public delegate void QuestCompletedHandler();
    public static event QuestCompletedHandler OnQuestCompleted;

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

    public static void QuestCompleted()
    {
        instance.dailyQuestsCompleted++;
        OnQuestCompleted?.Invoke();

        if (instance.dailyQuestsCompleted == instance._DailyQuestsAmount)
        {
            QuestSpecialPanelDisplay specialPanel = instance.QuestPanelsContainer.transform.GetChild(instance.QuestPanelsContainer.transform.childCount - 1).GetComponent<QuestSpecialPanelDisplay>();
            specialPanel.ActivateRewardButton();
        }
    }

    public int DailyQuestsAmount => _DailyQuestsAmount;

    [Serializable]
    public class QuestAndReward
    {
        public QuestSO quest;
        public int reward;
    }

    void Start()
    {
        SetQuestWindowVisible(true);
        GetNewQuests();
    }
    private void GetNewQuests()
    {
        List<QuestSO> availableQuests = LoadQuests();
        ActivateRandomQuests(availableQuests);
    }

    private List<QuestSO> LoadQuests()
    {
        QuestSO[] quests = Resources.LoadAll<QuestSO>(questDirectory);
        return new List<QuestSO>(quests);
    }

    private void ActivateRandomQuests(List<QuestSO> availableQuests)
    {
        for (int i = 0; i < _DailyQuestsAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableQuests.Count);
            QuestSO quest = availableQuests[randomIndex];
            quest.ResetProgress();
            quest.Activate();
            QuestAndReward lQR = new()
            {
                quest = quest,
                reward = QuestRewards[i]
            };
            activeQuests.Add(lQR);
            availableQuests.RemoveAt(randomIndex);
        }
    }

    private void ResetQuests()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            activeQuests[i].quest.Reset();
        }
        activeQuests.Clear();
    }

    public void SetQuestWindowVisible(bool pVisible)
    {
        QuestWindow.SetActive(pVisible);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetQuests();
            GetNewQuests();
        }
    }
}
