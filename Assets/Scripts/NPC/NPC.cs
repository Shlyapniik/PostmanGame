using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Character")]
    [SerializeField] private string relatedAddress;

    [Header("Dialogue")]
    [SerializeField] private DialogueData beforeDeliveryDialogue;
    [SerializeField] private DialogueData afterDeliveryDialogue;

    [Header("Dialogue Database")]
    [SerializeField] private NPCDialogueDatabase dialogueDatabase;

    [Header("Story")]
    [SerializeField] private NPCConditionalDialogue[] conditionalDialogues;
    [SerializeField] private NPCDayDialogue[] dayDialogues;

    [Header("Story Settings")]
    [SerializeField] private string deliveryFlag;
    [SerializeField] private NPCExclusiveChoiceGroup[] exclusiveChoiceGroups;

    private DialogueManager dialogueManager;
    private DialogueActionExecutor actionExecutor;
    private MailBag mailBag;
    private DayManager dayManager;
    private StoryFlags storyFlags;
    private QuestManager questManager;

    private void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        actionExecutor = FindFirstObjectByType<DialogueActionExecutor>();
        mailBag = FindFirstObjectByType<MailBag>();
        dayManager = FindFirstObjectByType<DayManager>();
        storyFlags = FindFirstObjectByType<StoryFlags>();
        questManager = FindFirstObjectByType<QuestManager>();
    }

    public bool CanInteract()
    {
        return dialogueManager != null &&
               !dialogueManager.IsDialogueActive;
    }

    public string GetInteractionText()
    {
        return "[E] Поговорить";
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        DialogueData dialogue = GetCurrentDialogue();

        if (dialogue != null)
            dialogueManager.StartDialogue(dialogue, HandleDialogueAction);
    }

    private DialogueData GetCurrentDialogue()
    {
        NPCConditionalDialogue[] conditions =
            dialogueDatabase != null
                ? dialogueDatabase.conditionalDialogues
                : conditionalDialogues;

        NPCDayDialogue[] days =
            dialogueDatabase != null
                ? dialogueDatabase.dayDialogues
                : dayDialogues;

        DialogueData beforeDelivery =
            dialogueDatabase != null
                ? dialogueDatabase.beforeDeliveryDialogue
                : beforeDeliveryDialogue;

        DialogueData afterDelivery =
            dialogueDatabase != null
                ? dialogueDatabase.afterDeliveryDialogue
                : afterDeliveryDialogue;

        // 1. Проверяем сюжетные диалоги
        if (conditions != null)
        {
            foreach (NPCConditionalDialogue entry in conditions)
            {
                if (IsConditionalDialogueAvailable(entry))
                    return entry.dialogue;
            }
        }

        // 2. Проверяем диалоги по дням
        if (dayManager != null && days != null)
        {
            foreach (NPCDayDialogue entry in days)
            {
                if (entry != null &&
                    entry.day == dayManager.CurrentDay &&
                    IsDialogueValid(entry.dialogue))
                {
                    return entry.dialogue;
                }
            }
        }

        // 3. Обычный разговор
        if (mailBag != null &&
            mailBag.WasMailDelivered(relatedAddress))
        {
            return afterDelivery;
        }

        return beforeDelivery;
    }

    private bool IsConditionalDialogueAvailable(NPCConditionalDialogue entry)
    {
        if (entry == null || !IsDialogueValid(entry.dialogue))
            return false;

        if (dayManager == null)
            return false;

        int currentDay = dayManager.CurrentDay;

        if (currentDay < entry.minimumDay)
            return false;

        if (entry.maximumDay > 0 &&
            currentDay > entry.maximumDay)
            return false;

        if (!AreRequiredFlagsSet(entry.requiredFlags))
            return false;

        if (!AreForbiddenFlagsAbsent(entry.forbiddenFlags))
            return false;

        if (!AreQuestConditionsMet(entry.questConditions))
            return false;

        return true;
    }

    private bool IsDialogueValid(DialogueData dialogue)
    {
        return dialogue != null &&
               dialogue.lines != null &&
               dialogue.lines.Length > 0;
    }

    private bool AreRequiredFlagsSet(string[] flags)
    {
        if (flags == null || flags.Length == 0)
            return true;

        if (storyFlags == null)
            return false;

        foreach (string flag in flags)
        {
            if (string.IsNullOrWhiteSpace(flag))
                continue;

            if (!storyFlags.HasFlag(flag))
                return false;
        }

        return true;
    }

    private bool AreForbiddenFlagsAbsent(string[] flags)
    {
        if (flags == null || flags.Length == 0)
            return true;

        if (storyFlags == null)
            return false;

        foreach (string flag in flags)
        {
            if (string.IsNullOrWhiteSpace(flag))
                continue;

            if (storyFlags.HasFlag(flag))
                return false;
        }

        return true;
    }
    private bool AreQuestConditionsMet(QuestCondition[] conditions)
    {
        if (conditions == null || conditions.Length == 0)
            return true;

        if (questManager == null)
            return false;

        foreach (QuestCondition condition in conditions)
        {
            if (condition == null ||
                string.IsNullOrWhiteSpace(condition.questId))
                return false;

            if (questManager.GetQuestData(condition.questId) == null)
                return false;

            QuestState currentState =
                questManager.GetQuestState(condition.questId);

            if (currentState != condition.requiredState)
                return false;
        }

        return true;
    }


    private void HandleDialogueAction(string actionId)
    {
        if (actionExecutor == null)
        {
            Debug.LogError($"NPC {name}: DialogueActionExecutor не найден.");
            return;
        }

        bool isDelivery = actionId == "deliver_registered_letter";

        bool hadRegisteredLetter = isDelivery &&
            mailBag != null &&
            mailBag.HasMailFor(relatedAddress, MailType.RegisteredLetter);

        actionExecutor.Execute(
            actionId,
            relatedAddress,
            deliveryFlag,
            exclusiveChoiceGroups);

        if (hadRegisteredLetter &&
            mailBag != null &&
            !mailBag.HasMailFor(relatedAddress, MailType.RegisteredLetter))
        {
            DialogueData afterDelivery =
                dialogueDatabase != null
                    ? dialogueDatabase.afterDeliveryDialogue
                    : afterDeliveryDialogue;

            if (IsDialogueValid(afterDelivery))
            {
                dialogueManager.StartDialogue(
                    afterDelivery,
                    HandleDialogueAction);
            }
        }
    }
}