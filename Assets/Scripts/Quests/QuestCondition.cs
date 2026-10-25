using System;
using UnityEngine;

[Serializable]
public class QuestCondition
{
    [Tooltip("ID задания из QuestData")]
    public string questId;

    [Tooltip("Состояние, необходимое для диалога")]
    public QuestState requiredState = QuestState.Active;
}