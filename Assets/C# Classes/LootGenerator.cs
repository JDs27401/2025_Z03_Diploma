using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LootGenerator : MonoBehaviour
{
    // Definicja typu źródła (Wrogowie lub Budynki)
    public enum SourceType { Enemy, Building }

    public ItemData GenerateLoot(List<ItemData> possibleItems, SourceType source)
    {
        // Losowanie liczby od 0 do 100
        float roll = Random.Range(0f, 100f);

        // KROK 1: Filtrowanie listy (Unusual nie może wypaść z wroga)
        // Używamy Linq (.Where), aby odrzucić niemożliwe przedmioty
        var validItems = possibleItems.Where(item => 
            source == SourceType.Building || item.rarity != ItemRarity.Unusual
        ).ToList();

        ItemRarity targetRarity = ItemRarity.Common;

        // KROK 2: Logika szans (Drzewko decyzyjne)
        if (source == SourceType.Enemy)
        {
            // Rare: ~5% szans z wroga
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
            // Unusual: >= 5% szans w budynku
            if (roll <= 5f) 
            {
                targetRarity = ItemRarity.Unusual;
            }
            // Rare: ~10-15% szans w budynku
            // Skoro 0-5 to Unusual, to 5-20 daje nam 15% szansy na Rare
            else if (roll <= 20f) 
            {
                targetRarity = ItemRarity.Rare;
            }
            else
            {
                targetRarity = ItemRarity.Common;
            }
        }
        
        // KROK 3: Wybór konkretnego przedmiotu z wylosowanej kategorii rzadkości
        var lootPool = validItems.Where(i => i.rarity == targetRarity).ToList();
        
        // Zabezpieczenie: Jeśli wylosowaliśmy rzadkość, której nie ma na liście (np. brak Rare w puli),
        // to spadamy do kategorii Common, żeby gracz nie dostał pustego loot boxa.
        if (lootPool.Count == 0)
        {
            lootPool = validItems.Where(i => i.rarity == ItemRarity.Common).ToList();
        }

        // Zwracamy losowy przedmiot z finalnej puli
        if (lootPool.Count > 0)
        {
            return lootPool[Random.Range(0, lootPool.Count)];
        }
        
        return null; // Brak przedmiotów do wyrzucenia
    }
}