using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Available Items")]
    [SerializeField] private ItemData[] availableItems;

    private readonly HashSet<string> ownedItems = new();

    public event Action OnInventoryChanged;

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        return ownedItems.Contains(itemId);
    }

    public bool AddItem(string itemId)
    {
        ItemData item = GetItemData(itemId);

        if (item == null)
        {
            Debug.LogWarning($"[Inventory] Предмет не найден: {itemId}");
            return false;
        }

        if (!ownedItems.Add(itemId))
            return false;

        Debug.Log($"[Inventory] Получен предмет: {item.itemName}");
        OnInventoryChanged?.Invoke();

        return true;
    }

    public bool RemoveItem(string itemId)
    {
        if (!ownedItems.Remove(itemId))
            return false;

        Debug.Log($"[Inventory] Предмет удалён: {itemId}");
        OnInventoryChanged?.Invoke();

        return true;
    }

    public ItemData GetItemData(string itemId)
    {
        if (availableItems == null ||
            string.IsNullOrWhiteSpace(itemId))
            return null;

        foreach (ItemData item in availableItems)
        {
            if (item != null && item.itemId == itemId)
                return item;
        }

        return null;
    }

    public List<ItemData> GetOwnedItems()
    {
        List<ItemData> result = new();

        foreach (string itemId in ownedItems)
        {
            ItemData item = GetItemData(itemId);

            if (item != null)
                result.Add(item);
        }

        return result;
    }
}