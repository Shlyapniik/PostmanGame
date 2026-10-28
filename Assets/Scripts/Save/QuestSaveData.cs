using System;

[Serializable]
public class QuestSaveData
{
    public string questId;
    public QuestState state;
    public int objectiveIndex;

    public QuestSaveData(
        string questId,
        QuestState state,
        int objectiveIndex)
    {
        this.questId = questId;
        this.state = state;
        this.objectiveIndex = objectiveIndex;
    }
}