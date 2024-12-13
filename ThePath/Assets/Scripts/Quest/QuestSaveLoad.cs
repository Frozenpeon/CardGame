using Com.IsartDigital.F2P.SO.QuestSO;
using UnityEngine;

public class QuestSaveLoad : MonoBehaviour
{
    private const string QUEST_TYPE_KEY_PREFIX = "QuestType_";
    private const string QUEST_ID_KEY_PREFIX = "QuestID_";
    private const string QUEST_REWARD_KEY_PREFIX = "QuestReward_";

    public static void SaveQuests(QuestManager.QuestAndReward[] quests)
    {
        PlayerPrefs.SetInt("QuestCount", quests.Length);

        for (int i = 0; i < quests.Length; i++)
        {
            PlayerPrefs.SetString(QUEST_TYPE_KEY_PREFIX + i, quests[i].quest.questType.ToString());
            PlayerPrefs.SetInt(QUEST_ID_KEY_PREFIX + i, quests[i].quest.ID);
            PlayerPrefs.SetInt(QUEST_REWARD_KEY_PREFIX + i, quests[i].reward);
        }

        PlayerPrefs.Save();
    }

    public static QuestManager.QuestAndReward[] LoadQuests()
    {
        int questCount = PlayerPrefs.GetInt("QuestCount", 0);
        QuestManager.QuestAndReward[] quests = new QuestManager.QuestAndReward[questCount];

        for (int i = 0; i < questCount; i++)
        {
            QuestType type = (QuestType)System.Enum.Parse(typeof(QuestType), PlayerPrefs.GetString(QUEST_TYPE_KEY_PREFIX + i));
            int id = PlayerPrefs.GetInt(QUEST_ID_KEY_PREFIX + i);
            int reward = PlayerPrefs.GetInt(QUEST_REWARD_KEY_PREFIX + i);

            // Find QuestSO by type and ID
            QuestSO quest = FindQuestByTypeAndID(type, id);

            if (quest != null)
            {
                quests[i] = new QuestManager.QuestAndReward { quest = quest, reward = reward };
            }
        }

        return quests;
    }

    private static QuestSO FindQuestByTypeAndID(QuestType type, int id)
    {
        // Implement your logic to find the QuestSO based on type and ID
        // For example, you might iterate through a list of available quests
        // and find the one that matches the given type and ID.
        // This depends on how your QuestSOs are structured and stored.
        // Return null if no matching quest is found.
        return null;
    }
}
