using UnityEngine;
using System.Collections.Generic;
using C__Classes.Singletons;

namespace C__Classes.Managers
{
    public class CraftingManager : SingletonNonPersistant<CraftingManager>
    {
        public List<RecipeData> allRecipes;

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
}