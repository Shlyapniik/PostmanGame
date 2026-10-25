using System;
using UnityEngine;

[Serializable]
public class NPCConditionalDialogue
{
    [Header("Day Conditions")]
    [Min(1)]
    public int minimumDay = 1;

    [Tooltip("0 = без ограничения")]
    [Min(0)]
    public int maximumDay = 0;

    [Header("Required Flags")]
    public string[] requiredFlags;

    [Header("Forbidden Flags")]
    public string[] forbiddenFlags;

    [Header("Quest Conditions")]
    [Tooltip("Все условия должны выполняться")]
    public QuestCondition[] questConditions;

    [Header("Dialogue")]
    public DialogueData dialogue;
}