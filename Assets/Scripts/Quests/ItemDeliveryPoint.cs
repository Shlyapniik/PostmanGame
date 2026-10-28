using UnityEngine;

public class ItemDeliveryPoint : MonoBehaviour, IInteractable
{
    [Header("Required Item")]
    [SerializeField] private string requiredItemId;

    [Header("Quest")]
    [SerializeField] private string questId;
    [SerializeField] private string objectiveId;

    [Header("Interaction")]
    [SerializeField]
    private string interactionText =
        "[E] Передать предмет";

    [Header("Story")]
    [SerializeField] private string flagOnDelivery;

    private InventoryManager inventoryManager;
    private QuestManager questManager;
    private StoryFlags storyFlags;

    private void Start()
    {
        inventoryManager =
            FindFirstObjectByType<InventoryManager>();

        questManager =
            FindFirstObjectByType<QuestManager>();

        storyFlags =
            FindFirstObjectByType<StoryFlags>();

        if (inventoryManager == null)
        {
            Debug.LogError(
                $"ItemDeliveryPoint {name}: InventoryManager не найден.");
        }

        if (questManager == null)
        {
            Debug.LogError(
                $"ItemDeliveryPoint {name}: QuestManager не найден.");
        }
    }

    public bool CanInteract()
    {
        if (inventoryManager == null ||
            questManager == null)
        {
            return false;
        }

        if (!inventoryManager.HasItem(
            requiredItemId))
        {
            return false;
        }

        if (questManager.GetQuestState(questId) !=
            QuestState.Active)
        {
            return false;
        }

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

        bool itemRemoved =
            inventoryManager.RemoveItem(
                requiredItemId);

        if (!itemRemoved)
            return;

        bool objectiveCompleted =
            questManager.CompleteObjective(
                questId,
                objectiveId);

        if (!objectiveCompleted)
        {
            inventoryManager.AddItem(
                requiredItemId);

            return;
        }

        if (!string.IsNullOrWhiteSpace(
            flagOnDelivery))
        {
            storyFlags?.SetFlag(
                flagOnDelivery);
        }

        Debug.Log(
            $"[ItemDelivery] Предмет {requiredItemId} успешно передан.");
    }
}