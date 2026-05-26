using UnityEngine;
using UnityEngine.EventSystems;
using C__Classes.Managers;

public class RecipeSlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData currentItem;
    private bool isDiscovered;

    public void SetupSlot(ItemData item, bool discovered)
    {
        currentItem = item;
        isDiscovered = discovered;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(currentItem, isDiscovered, false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    private void OnDisable()
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}