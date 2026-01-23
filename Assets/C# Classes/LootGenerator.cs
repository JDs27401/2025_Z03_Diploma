using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LootGenerator : MonoBehaviour
{
    public enum SourceType { Enemy, Building }

    public ItemData GenerateLoot(List<ItemData> possibleItems, SourceType source)
    {
        float roll = Random.Range(0f, 100f);

        var validItems = possibleItems.Where(item => 
            source == SourceType.Building || item.rarity != ItemRarity.Unusual
        ).ToList();

        ItemRarity targetRarity = ItemRarity.Common;

        if (source == SourceType.Enemy)
        {
            if (roll <= 5f) 
            {
                targetRarity = ItemRarity.Rare;
            }
            else
            {
                targetRarity = ItemRarity.Common;
            }
        } 
        else if (source == SourceType.Building)
        {
            if (roll <= 5f) 
            {
                targetRarity = ItemRarity.Unusual;
            }
            else if (roll <= 20f) 
            {
                targetRarity = ItemRarity.Rare;
            }
            else
            {
                targetRarity = ItemRarity.Common;
            }
        }
        
        var lootPool = validItems.Where(i => i.rarity == targetRarity).ToList();
        
        if (lootPool.Count == 0)
        {
            lootPool = validItems.Where(i => i.rarity == ItemRarity.Common).ToList();
        }

        if (lootPool.Count > 0)
        {
            return lootPool[Random.Range(0, lootPool.Count)];
        }
        
        return null;
    }
}