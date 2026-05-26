using UnityEngine;
using C__Classes.Managers;

public class RecipeListUI : MonoBehaviour
{
    [Header("References")]
    public GameObject recipeEntryPrefab;
    public Transform contentContainer;

    private void OnEnable()
    {
        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        if (CraftingManager.Instance == null || ItemDiscoveryManager.Instance == null) return;

        foreach (RecipeData recipe in CraftingManager.Instance.allRecipes)
        {
            if (ShouldShowRecipe(recipe))
            {
                GameObject newEntry = Instantiate(recipeEntryPrefab, contentContainer);
                RecipeEntryUI entryUI = newEntry.GetComponent<RecipeEntryUI>();
                entryUI.Setup(recipe);
            }
        }
    }

    private bool ShouldShowRecipe(RecipeData recipe)
    {
        foreach (ItemData input in recipe.inputs)
        {
            if (input != null && ItemDiscoveryManager.Instance.IsItemDiscovered(input.id))
            {
                return true;
            }
        }
        return false;
    }
}