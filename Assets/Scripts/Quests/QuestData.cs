using UnityEngine;

[CreateAssetMenu(
    fileName = "NewQuest",
    menuName = "Postman/Quest")]
public class QuestData : ScriptableObject
{
    [Header("Identity")]
    public string questId;
    public string questName;

    [TextArea(2, 5)]
    public string description;

    [Header("Objectives")]
    public QuestObjective[] objectives;
}