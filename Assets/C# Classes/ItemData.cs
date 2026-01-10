using UnityEngine;

public enum ItemRarity
{
    Common,
    Rare,
    Unusual,
    Unique
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public ItemRarity rarity;
    
    [TextArea]
    public string description;
    
    public bool isStackable = true;
}