using UnityEngine;

public enum ItemRarity
{
    Common,
    Rare,
    Unusual,
    Unique
}

public enum ItemType
{
    General,    
    Ammo9mm,    
    Ammo12Gauge,
    Collectible
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public ItemRarity rarity;
    public ItemType itemType;
    
    [TextArea]
    public string description;
    
    [Header("Stackowanie")]
    public bool isStackable = true;
    
    public int maxStackSize = 100;
}