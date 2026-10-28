using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int currentDay;
    public WorkDayState currentDayState;

    public int deliveredTodayCount;
    public int lastDayDeliveredCount;
    public int lastDayRemainingCount;

    public float playerPositionX;
    public float playerPositionY;

    public List<string> storyFlags = new();
    public List<string> inventoryItemIds = new();
    public List<QuestSaveData> quests = new();

    public List<MailItemSaveData> currentMail = new();
    public List<MailItemSaveData> deliveredMail = new();

    public SaveData()
    {
        currentDay = 1;
        currentDayState = WorkDayState.Morning;

        playerPositionX = 0f;
        playerPositionY = 0f;
    }

    public Vector2 GetPlayerPosition()
    {
        return new Vector2(
            playerPositionX,
            playerPositionY);
    }

    public void SetPlayerPosition(Vector2 position)
    {
        playerPositionX = position.x;
        playerPositionY = position.y;
    }
}