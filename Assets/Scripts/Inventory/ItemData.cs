using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "Postman/Inventory Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string itemName;

    [TextArea(2, 5)]
    public string description;

    [Header("Visual")]
    public Sprite icon;
}