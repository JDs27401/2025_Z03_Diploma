using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler
{
    // Czy to slot siatki craftingu (sprawdzający przepis), czy zwykły plecak?
    public bool isCraftingGridSlot = false;
    public CraftingUI craftingUI; // Jeśli to slot craftingu, przypisz tu Manager UI

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            draggableItem.parentAfterDrag = transform;
            
            // // --- DIAGNOSTYKA START ---
            // Debug.Log($"Upuszczono na slot: {gameObject.name}");
            
            // if (!isCraftingGridSlot)
            // {
            //     Debug.LogWarning("-> Ignoruję przepis: To nie jest slot craftingu (odznaczony checkbox 'Is Crafting Grid Slot')");
            // }
            
            // if (craftingUI == null)
            // {
            //     Debug.LogError("-> BŁĄD: Nie przypisałeś 'Crafting UI' w Inspectorze tego slota!");
            // }
            // // --- DIAGNOSTYKA KONIEC ---

            if (isCraftingGridSlot && craftingUI != null)
            {
                Debug.Log("-> Warunki spełnione. Powiadamiam UI za 0.05s...");
                Invoke(nameof(NotifyUI), 0.05f);
            }
        }
    }

    private void NotifyUI()
    {
        craftingUI.UpdateCraftingGrid();
    }
}