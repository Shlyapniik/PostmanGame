using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotStarted,
    Active,
    Completed
}

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestData[] availableQuests;

    private readonly Dictionary<string, QuestState> questStates = new();
    private readonly Dictionary<string, int> objectiveIndices = new();

    public event Action<string, QuestState> OnQuestStateChanged;
    public event Action<string, int> OnQuestObjectiveChanged;

    public QuestState GetQuestState(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return QuestState.NotStarted;

        return questStates.TryGetValue(questId, out QuestState state)
            ? state
            : QuestState.NotStarted;
    }

    public QuestData GetQuestData(string questId)
    {
        if (availableQuests == null)
            return null;

        foreach (QuestData quest in availableQuests)
        {
            if (quest != null && quest.questId == questId)
                return quest;
        }

        return null;
    }

    public bool StartQuest(string questId)
    {
        QuestData quest = GetQuestData(questId);

        if (quest == null)
        {
            Debug.LogWarning($"Задание не найдено: {questId}");
            return false;
        }

        if (GetQuestState(questId) != QuestState.NotStarted)
            return false;

        questStates[questId] = QuestState.Active;
        objectiveIndices[questId] = 0;

        Debug.Log($"[Quest] Задание начато: {questId}");
        OnQuestStateChanged?.Invoke(questId, QuestState.Active);
        OnQuestObjectiveChanged?.Invoke(questId, 0);

        return true;
    }

    public bool CompleteObjective(string questId, string objectiveId)
    {
        if (GetQuestState(questId) != QuestState.Active)
            return false;

        QuestData quest = GetQuestData(questId);

        if (quest == null ||
            quest.objectives == null ||
            quest.objectives.Length == 0)
            return false;

        int currentIndex = GetCurrentObjectiveIndex(questId);

        if (currentIndex >= quest.objectives.Length)
            return false;

        QuestObjective currentObjective = quest.objectives[currentIndex];

        if (currentObjective == null ||
            currentObjective.objectiveId != objectiveId)
            return false;

        currentIndex++;
        objectiveIndices[questId] = currentIndex;

        Debug.Log($"[Quest] Цель выполнена: {objectiveId}");

        if (currentIndex >= quest.objectives.Length)
        {
            CompleteQuest(questId);
        }
        else
        {
            OnQuestObjectiveChanged?.Invoke(questId, currentIndex);
        }

        return true;
    }

    public bool CompleteQuest(string questId)
    {
        if (GetQuestState(questId) != QuestState.Active)
            return false;

        questStates[questId] = QuestState.Completed;

        Debug.Log($"[Quest] Задание завершено: {questId}");
        OnQuestStateChanged?.Invoke(questId, QuestState.Completed);

        return true;
    }

    public int GetCurrentObjectiveIndex(string questId)
    {
        return objectiveIndices.TryGetValue(questId, out int index)
            ? index
            : 0;
    }

    public QuestObjective GetCurrentObjective(string questId)
    {
        if (GetQuestState(questId) != QuestState.Active)
            return null;

        QuestData quest = GetQuestData(questId);

        if (quest == null || quest.objectives == null)
            return null;

        int index = GetCurrentObjectiveIndex(questId);

        if (index < 0 || index >= quest.objectives.Length)
            return null;

        return quest.objectives[index];
    }

    public List<QuestData> GetActiveQuests()
    {
        List<QuestData> result = new();

        if (availableQuests == null)
            return result;

        foreach (QuestData quest in availableQuests)
        {
            if (quest != null &&
                GetQuestState(quest.questId) == QuestState.Active)
            {
                result.Add(quest);
            }
        }

        return result;
    }
}