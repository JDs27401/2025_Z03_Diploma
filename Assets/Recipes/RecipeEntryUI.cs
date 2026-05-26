using UnityEngine;
using UnityEngine.UI;
using C__Classes.Managers;

public class RecipeEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public Image resultIcon;
    public Image[] inputIcons;

    public void Setup(RecipeData recipe)
    {
        resultIcon.sprite = recipe.resultItem.icon;
        
        bool allIngredientsDiscovered = true;

        for (int i = 0; i < 4; i++)
        {
            ItemData inputItem = recipe.inputs[i];
            
            RecipeSlotHover hoverComponent = inputIcons[i].GetComponent<RecipeSlotHover>();
            if (hoverComponent == null) hoverComponent = inputIcons[i].gameObject.AddComponent<RecipeSlotHover>();

            if (inputItem != null)
            {
                inputIcons[i].gameObject.SetActive(true);
                inputIcons[i].sprite = inputItem.icon;

                bool isDiscovered = ItemDiscoveryManager.Instance.IsItemDiscovered(inputItem.id);

                if (isDiscovered)
                {
                    inputIcons[i].color = Color.white;
                }
                else
                {
                    inputIcons[i].color = Color.black;
                    allIngredientsDiscovered = false;
                }
                
                hoverComponent.SetupSlot(inputItem, isDiscovered);
            }
            else
            {
                inputIcons[i].gameObject.SetActive(false);
                hoverComponent.SetupSlot(null, false);
            }
        }

        RecipeSlotHover resultHoverComponent = resultIcon.GetComponent<RecipeSlotHover>();
        if (resultHoverComponent == null) resultHoverComponent = resultIcon.gameObject.AddComponent<RecipeSlotHover>();
        
        resultHoverComponent.SetupSlot(recipe.resultItem, allIngredientsDiscovered);

        if (allIngredientsDiscovered)
        {
            resultIcon.color = Color.white;
        }
        else
        {
            resultIcon.color = Color.black;
        }
    }
}