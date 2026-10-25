using UnityEngine;

[CreateAssetMenu(
    fileName = "NewNPCDialogueDatabase",
    menuName = "Postman/NPC Dialogue Database")]
public class NPCDialogueDatabase : ScriptableObject
{
    [Header("Default Dialogues")]
    public DialogueData beforeDeliveryDialogue;
    public DialogueData afterDeliveryDialogue;

    [Header("Story Dialogues")]
    public NPCConditionalDialogue[] conditionalDialogues;

    [Header("Day Dialogues")]
    public NPCDayDialogue[] dayDialogues;
}