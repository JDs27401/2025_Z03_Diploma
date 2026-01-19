using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    // Singleton zostawiamy, bo CraftingUI go potrzebuje do odwołań
    public static CraftingManager instance;

    public List<RecipeData> allRecipes;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    // Ta metoda tylko zwraca ItemData (logika), nie dotyka UI!
    public ItemData CheckForRecipe(ItemData[] currentGrid)
    {
        foreach (var recipe in allRecipes)
        {
            if (MatchRecipe(recipe, currentGrid))
            {
                return recipe.resultItem;
            }
        }
        return null;
    }

    private bool MatchRecipe(RecipeData recipe, ItemData[] currentGrid)
    {
        for (int i = 0; i < 4; i++)
        {
            ItemData recipeItem = recipe.inputs[i];
            ItemData gridItem = (i < currentGrid.Length) ? currentGrid[i] : null;

            bool recipeSlotEmpty = recipeItem == null;
            bool gridSlotEmpty = gridItem == null;

            if (recipeSlotEmpty && gridSlotEmpty) continue;
            if (recipeSlotEmpty != gridSlotEmpty) return false;
            if (recipeItem.itemName != gridItem.itemName) return false;
        }
        return true;
    }
}