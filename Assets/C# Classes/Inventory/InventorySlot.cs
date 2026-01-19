using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Konfiguracja")]
    // ZAZNACZ 'TRUE' W UNITY DLA SLOTU WYNIKU (RESULT SLOT)!
    public bool isReadOnly = false; 

    [Header("Dane (Nie ruszaj w edytorze)")]
    public ItemData currentItem;
    public Image iconDisplay;

    public void OnDrop(PointerEventData eventData)
    {
        // 1. BLOKADA: Jeśli slot jest tylko do odczytu (np. Wynik), nie pozwól nic wrzucić
        if (isReadOnly) return;

        if (eventData.pointerDrag == null) return;

        CleanUpGhosts();

        GameObject droppedObj = eventData.pointerDrag;
        DraggableItem droppedItemScript = droppedObj.GetComponent<DraggableItem>();

        if (droppedItemScript == null) return;

        Transform oldSlotTransform = droppedItemScript.parentAfterDrag;

        // --- SCENARIUSZ A: SLOT PUSTY ---
        if (transform.childCount == 0)
        {
            droppedItemScript.parentAfterDrag = transform;

            this.currentItem = droppedItemScript.itemData;
            this.iconDisplay = droppedObj.GetComponent<Image>();

            UpdateOldSlot(oldSlotTransform, null, null);
        }
        // --- SCENARIUSZ B: ZAMIANA (SWAP) ---
        else
        {
            GameObject residentObj = transform.GetChild(0).gameObject;
            DraggableItem residentItemScript = residentObj.GetComponent<DraggableItem>();

            if (residentItemScript != null)
            {
                residentItemScript.transform.SetParent(oldSlotTransform);
                residentItemScript.parentAfterDrag = oldSlotTransform;
                residentItemScript.transform.localPosition = Vector3.zero;
                residentItemScript.transform.localScale = Vector3.one;

                droppedItemScript.parentAfterDrag = transform;

                this.currentItem = droppedItemScript.itemData;
                this.iconDisplay = droppedObj.GetComponent<Image>();

                UpdateOldSlot(oldSlotTransform, residentItemScript.itemData, residentObj.GetComponent<Image>());
            }
        }
        
        TooltipManager.instance.HideTooltip();
    }

    private void UpdateOldSlot(Transform oldSlotTransform, ItemData newItem, Image newIcon)
    {
        // Czy to był Ekwipunek?
        InventorySlot invSlot = oldSlotTransform.GetComponent<InventorySlot>();
        if (invSlot != null)
        {
            invSlot.currentItem = newItem;
            invSlot.iconDisplay = newIcon;
            return; 
        }

        // Czy to był Crafting?
        CraftingSlot craftSlot = oldSlotTransform.GetComponent<CraftingSlot>();
        if (craftSlot != null)
        {
            craftSlot.currentItem = newItem;
            craftSlot.iconDisplay = newIcon;

            // WAŻNE: Powiadom UI craftingu, że zabrano przedmiot!
            if (CraftingUI.instance != null)
            {
                CraftingUI.instance.UpdateCraftingGrid();
            }
            return;
        }
    }

    private void CleanUpGhosts()
    {
        if (transform.childCount > 0)
        {
            GameObject currentObject = transform.GetChild(0).gameObject;
            Image objectImage = currentObject.GetComponent<Image>();
            
            if (objectImage == null || objectImage.sprite == null || !currentObject.activeSelf)
            {
                DestroyImmediate(currentObject);
            }
        }
    }
}