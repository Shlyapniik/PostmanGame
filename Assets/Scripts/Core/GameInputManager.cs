using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputBlocker
{
    Dialogue,
    MailBag,
    QuestJournal,
    DaySummary
}

public class GameInputManager : MonoBehaviour
{
    private readonly HashSet<InputBlocker> blockers = new();

    public bool IsPlayerInputBlocked => blockers.Count > 0;

    public event Action<bool> OnInputBlockChanged;

    public void AddBlocker(InputBlocker blocker)
    {
        bool wasBlocked = IsPlayerInputBlocked;

        blockers.Add(blocker);

        if (wasBlocked != IsPlayerInputBlocked)
            OnInputBlockChanged?.Invoke(IsPlayerInputBlocked);
    }

    public void RemoveBlocker(InputBlocker blocker)
    {
        bool wasBlocked = IsPlayerInputBlocked;

        blockers.Remove(blocker);

        if (wasBlocked != IsPlayerInputBlocked)
            OnInputBlockChanged?.Invoke(IsPlayerInputBlocked);
    }

    public bool HasBlocker(InputBlocker blocker)
    {
        return blockers.Contains(blocker);
    }
}