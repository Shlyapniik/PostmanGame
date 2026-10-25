using System;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string text;

    [Tooltip("-1 = закончить диалог")]
    public int nextLineIndex = -1;

    [Tooltip("Необязательное игровое действие")]
    public string actionId;

    [Tooltip("Дополнительные действия, выполняемые после основного")]
    public string[] additionalActionIds;
}

[Serializable]
public class DialogueData
{
    public string speakerName;

    [TextArea(2, 5)]
    public string[] lines;

    public DialogueChoice[] choices;
}