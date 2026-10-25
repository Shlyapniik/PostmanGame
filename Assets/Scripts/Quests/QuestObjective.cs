using System;
using UnityEngine;

[Serializable]
public class QuestObjective
{
    [Tooltip("Уникальный ID цели внутри задания")]
    public string objectiveId;

    [TextArea(2, 4)]
    public string description;
}