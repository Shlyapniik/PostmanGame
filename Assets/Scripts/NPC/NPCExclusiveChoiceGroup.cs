using System;
using UnityEngine;

[Serializable]
public class NPCExclusiveChoiceGroup
{
    [Tooltip("Название группы для удобства")]
    public string groupName;

    [Tooltip("Флаги, из которых можно установить только один")]
    public string[] flagIds;
}