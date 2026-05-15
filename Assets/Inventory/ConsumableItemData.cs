using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Item/Consumable Item")]
public class ConsumableItemData : ItemData
{
    [Header("Consumable Effects")]
    public List<ConsumableEffectData> effects = new List<ConsumableEffectData>();

    private void OnValidate()
    {
        itemType = ItemType.Consumable;
        isStackable = true;
        if (maxStackSize < 1)
        {
            maxStackSize = 1;
        }
    }
}

