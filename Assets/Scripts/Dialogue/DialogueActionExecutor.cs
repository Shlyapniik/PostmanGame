using UnityEngine;

public class DialogueActionExecutor : MonoBehaviour
{
    private StoryFlags storyFlags;
    private MailBag mailBag;
    private QuestManager questManager;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        storyFlags = FindFirstObjectByType<StoryFlags>();
        mailBag = FindFirstObjectByType<MailBag>();
        questManager = FindFirstObjectByType<QuestManager>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void Execute(string actionId, string relatedAddress,
        string deliveryFlag, NPCExclusiveChoiceGroup[] exclusiveGroups)
    {
        if (string.IsNullOrWhiteSpace(actionId))
            return;

        if (actionId == "deliver_registered_letter")
        {
            DeliverRegisteredLetter(relatedAddress, deliveryFlag);
            return;
        }

        if (actionId.StartsWith("set_flag:"))
        {
            string flagId = actionId.Substring("set_flag:".Length);

            if (CanSetFlag(flagId, exclusiveGroups))
                storyFlags?.SetFlag(flagId);

            return;
        }

        if (actionId.StartsWith("remove_flag:"))
        {
            string flagId = actionId.Substring("remove_flag:".Length);
            storyFlags?.RemoveFlag(flagId);
            return;
        }

        if (actionId.StartsWith("start_quest:"))
        {
            string questId = actionId.Substring("start_quest:".Length);
            questManager?.StartQuest(questId);
            return;
        }

        if (actionId.StartsWith("complete_quest:"))
        {
            string questId = actionId.Substring("complete_quest:".Length);
            questManager?.CompleteQuest(questId);
            return;
        }

        if (actionId.StartsWith("complete_objective:"))
        {
            string parameters = actionId.Substring(
                "complete_objective:".Length);

            string[] parts = parameters.Split(':', 2);

            if (parts.Length != 2)
            {
                Debug.LogWarning(
                    $"Неверный формат действия: {actionId}");
                return;
            }

            questManager?.CompleteObjective(parts[0], parts[1]);
            return;
        }

        if (actionId.StartsWith("give_item:"))
        {
            string itemId = actionId.Substring("give_item:".Length);
            inventoryManager?.AddItem(itemId);
            return;
        }

        if (actionId.StartsWith("remove_item:"))
        {
            string itemId = actionId.Substring("remove_item:".Length);
            inventoryManager?.RemoveItem(itemId);
            return;
        }

        Debug.LogWarning($"Неизвестное действие диалога: {actionId}");
    }

    private void DeliverRegisteredLetter(string address, string deliveryFlag)
    {
        if (mailBag == null)
            return;

        bool delivered = mailBag.DeliverMail(
            address, MailType.RegisteredLetter);

        if (!delivered)
            return;

        if (!string.IsNullOrWhiteSpace(deliveryFlag))
            storyFlags?.SetFlag(deliveryFlag);
    }

    private bool CanSetFlag(
        string flagId,
        NPCExclusiveChoiceGroup[] exclusiveGroups)
    {
        if (storyFlags == null || string.IsNullOrWhiteSpace(flagId))
            return false;

        if (exclusiveGroups == null)
            return true;

        foreach (NPCExclusiveChoiceGroup group in exclusiveGroups)
        {
            if (group == null || group.flagIds == null)
                continue;

            bool belongsToGroup = false;

            foreach (string groupFlag in group.flagIds)
            {
                if (groupFlag == flagId)
                {
                    belongsToGroup = true;
                    break;
                }
            }

            if (!belongsToGroup)
                continue;

            foreach (string groupFlag in group.flagIds)
            {
                if (storyFlags.HasFlag(groupFlag))
                    return false;
            }
        }

        return true;
    }
}