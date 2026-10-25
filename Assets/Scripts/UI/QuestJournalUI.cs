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
    private DialogueManager dialogueManager;
    private DayManager dayManager;

    private void Start()
    {
        questManager = FindFirstObjectByType<QuestManager>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        dayManager = FindFirstObjectByType<DayManager>();

        if (questManager == null)
        {
            Debug.LogError("QuestJournalUI: QuestManager не найден.");
            return;
        }

        questManager.OnQuestStateChanged += HandleQuestChanged;
        questManager.OnQuestObjectiveChanged += HandleObjectiveChanged;

        journalPanel.SetActive(false);
        RefreshJournal();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        bool isOpen = journalPanel.activeSelf;

        if (!isOpen)
        {
            if (dialogueManager != null &&
                dialogueManager.IsDialogueActive)
                return;

            if (dayManager != null &&
                dayManager.CurrentState == WorkDayState.Summary)
                return;
        }

        journalPanel.SetActive(!isOpen);

        if (!isOpen)
            RefreshJournal();
    }

    private void HandleQuestChanged(string questId, QuestState state)
    {
        RefreshJournal();
    }

    private void RefreshJournal()
    {
        if (questManager == null || questListText == null)
            return;

        var quests = questManager.GetActiveQuests();

        if (quests.Count == 0)
        {
            questListText.text = "Нет активных заданий.";
            return;
        }

        StringBuilder builder = new();

        foreach (QuestData quest in quests)
        {
            builder.AppendLine($"<b>{quest.questName}</b>");
            builder.AppendLine(quest.description);

            QuestObjective objective =
                questManager.GetCurrentObjective(quest.questId);

            if (objective != null)
            {
                builder.AppendLine();
                builder.AppendLine(
                    $"<color=#FFD966>Текущая цель:</color> {objective.description}");
            }

            builder.AppendLine();
            builder.AppendLine();
        }

        questListText.text = builder.ToString();
    }

    private void OnDestroy()
    {
        if (questManager == null)
            return;

        questManager.OnQuestStateChanged -= HandleQuestChanged;
        questManager.OnQuestObjectiveChanged -= HandleObjectiveChanged;
    }

    private void HandleObjectiveChanged(string questId, int objectiveIndex)
    {
        RefreshJournal();
    }
}