using UnityEngine;

public class QuestTrigger : MonoBehaviour, IInteractable
{
    [Header("Quest")]
    [SerializeField] private string questId;
    [SerializeField] private string objectiveId;

    [Header("Interaction")]
    [SerializeField] private string interactionText = "[E] Осмотреть";

    [Tooltip("Отключить взаимодействие после выполнения цели")]
    [SerializeField] private bool disableAfterCompletion = true;

    [Header("Story")]
    [Tooltip("Необязательный флаг после выполнения цели")]
    [SerializeField] private string flagOnCompletion;

    private QuestManager questManager;
    private StoryFlags storyFlags;

    private bool hasBeenActivated;

    private void Start()
    {
        questManager = FindFirstObjectByType<QuestManager>();
        storyFlags = FindFirstObjectByType<StoryFlags>();

        if (questManager == null)
            Debug.LogError($"QuestTrigger {name}: QuestManager не найден.");
    }

    public bool CanInteract()
    {
        if (questManager == null)
            return false;

        if (disableAfterCompletion && hasBeenActivated)
            return false;

        if (questManager.GetQuestState(questId) != QuestState.Active)
            return false;

        QuestObjective objective =
            questManager.GetCurrentObjective(questId);

        return objective != null &&
               objective.objectiveId == objectiveId;
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        bool completed = questManager.CompleteObjective(
            questId,
            objectiveId);

        if (!completed)
            return;

        hasBeenActivated = true;

        if (!string.IsNullOrWhiteSpace(flagOnCompletion))
            storyFlags?.SetFlag(flagOnCompletion);

        Debug.Log($"[QuestTrigger] Объект {name} активирован.");
    }
}