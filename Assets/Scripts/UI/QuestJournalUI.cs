using System.Text;
using TMPro;
using UnityEngine;

public class QuestJournalUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private TMP_Text questListText;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.J;

    private QuestManager questManager;
    private GameInputManager inputManager;

    private void Start()
    {
        questManager =
            FindFirstObjectByType<QuestManager>();

        inputManager =
            FindFirstObjectByType<GameInputManager>();

        if (questManager == null)
        {
            Debug.LogError(
                "QuestJournalUI: QuestManager не найден.");
            return;
        }

        if (inputManager == null)
        {
            Debug.LogError(
                "QuestJournalUI: GameInputManager не найден.");
        }

        questManager.OnQuestStateChanged +=
            HandleQuestChanged;

        questManager.OnQuestObjectiveChanged +=
            HandleObjectiveChanged;

        journalPanel.SetActive(false);

        RefreshJournal();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        if (journalPanel.activeSelf)
        {
            CloseJournal();
            return;
        }

        if (inputManager != null &&
            inputManager.IsPlayerInputBlocked)
        {
            return;
        }

        OpenJournal();
    }

    private void OpenJournal()
    {
        journalPanel.SetActive(true);

        inputManager?.AddBlocker(
            InputBlocker.QuestJournal);

        RefreshJournal();
    }

    private void CloseJournal()
    {
        journalPanel.SetActive(false);

        inputManager?.RemoveBlocker(
            InputBlocker.QuestJournal);
    }

    private void HandleQuestChanged(
        string questId,
        QuestState state)
    {
        RefreshJournal();
    }

    private void HandleObjectiveChanged(
        string questId,
        int objectiveIndex)
    {
        RefreshJournal();
    }

    private void RefreshJournal()
    {
        if (questManager == null ||
            questListText == null)
        {
            return;
        }

        var quests =
            questManager.GetActiveQuests();

        if (quests.Count == 0)
        {
            questListText.text =
                "Нет активных заданий.";

            return;
        }

        StringBuilder builder = new();

        foreach (QuestData quest in quests)
        {
            builder.AppendLine(
                $"<b>{quest.questName}</b>");

            builder.AppendLine(
                quest.description);

            QuestObjective objective =
                questManager.GetCurrentObjective(
                    quest.questId);

            if (objective != null)
            {
                builder.AppendLine();

                builder.AppendLine(
                    $"<color=#FFD966>Текущая цель:</color> " +
                    objective.description);
            }

            builder.AppendLine();
            builder.AppendLine();
        }

        questListText.text =
            builder.ToString();
    }

    private void OnDestroy()
    {
        if (questManager == null)
            return;

        questManager.OnQuestStateChanged -=
            HandleQuestChanged;

        questManager.OnQuestObjectiveChanged -=
            HandleObjectiveChanged;
    }
}