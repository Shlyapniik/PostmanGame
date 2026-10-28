using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryFlags : MonoBehaviour
{
    private readonly HashSet<string> flags = new();

    public event Action<string, bool> OnFlagChanged;

    public bool HasFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
            return false;

        return flags.Contains(flagId);
    }

    public void SetFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
            return;

        if (!flags.Add(flagId))
            return;

        Debug.Log($"[Story] Флаг установлен: {flagId}");

        OnFlagChanged?.Invoke(
            flagId,
            true);
    }

    public void RemoveFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
            return;

        if (!flags.Remove(flagId))
            return;

        Debug.Log($"[Story] Флаг удалён: {flagId}");

        OnFlagChanged?.Invoke(
            flagId,
            false);
    }

    public List<string> GetAllFlags()
    {
        return new List<string>(flags);
    }

    public void LoadFlags(List<string> savedFlags)
    {
        flags.Clear();

        if (savedFlags == null)
            return;

        foreach (string flagId in savedFlags)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
                flags.Add(flagId);
        }
    }
}