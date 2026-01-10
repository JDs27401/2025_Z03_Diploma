using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CraftingManager : MonoBehaviour
{
    public List<RecipeData> allRecipes;

    public ItemData CheckForRecipe(ItemData[] currentGrid)
    {
        foreach (var recipe in allRecipes)
        {
            if (MatchRecipe(recipe, currentGrid))
            {
                Debug.Log($"Wytworzono: {recipe.resultItem.itemName}");
                return recipe.resultItem;
            }
        }
        return null;
    }

    private bool MatchRecipe(RecipeData recipe, ItemData[] currentGrid)
    {
        Debug.Log($"--- Sprawdzam przepis: {recipe.recipeName} ---");

        for (int i = 0; i < 4; i++)
        {
            ItemData recipeItem = recipe.inputs[i];
            ItemData gridItem = currentGrid[i];

            string rName = recipeItem != null ? recipeItem.name : "PUSTO";
            string gName = gridItem != null ? gridItem.name : "PUSTO";

            // Sprawdźmy co widzi system w tym slocie
            Debug.Log($"Slot {i}: Przepis chce [{rName}] <-> Na stole jest [{gName}]");

            bool recipeSlotEmpty = recipeItem == null;
            bool gridSlotEmpty = gridItem == null;

            // 1. Jeśli oba puste -> OK
            if (recipeSlotEmpty && gridSlotEmpty) continue;

            // 2. Jeśli jedno puste a drugie nie -> BŁĄD
            if (recipeSlotEmpty != gridSlotEmpty)
            {
                Debug.Log($"-> BŁĄD w slocie {i}: Niezgodność pustego pola! (Wymagane: {rName}, Jest: {gName})");
                return false;
            }

            // 3. Jeśli różne przedmioty -> BŁĄD
            if (recipeItem != gridItem)
            {
                Debug.Log($"-> BŁĄD w slocie {i}: Zły przedmiot! (Wymagane: {rName}, Jest: {gName})");
                return false;
            }
        }
        
        Debug.Log("-> SUKCES! Przepis pasuje idealnie.");
        return true;
    }
}