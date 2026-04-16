using C__Classes.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour, IDropHandler
{
    public ItemData currentItem;
    public Image iconDisplay;
    public int currentCount = 0; 

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        CleanUpGhosts();

        GameObject droppedObj = eventData.pointerDrag;
        DraggableItem droppedItemScript = droppedObj.GetComponent<DraggableItem>();

        if (droppedItemScript == null) return;

        Transform oldSlotTransform = droppedItemScript.parentAfterDrag;

        if (currentItem != null && currentItem == droppedItemScript.itemData && currentItem.isStackable)
        {
            if (oldSlotTransform == transform) return;

            int spaceLeft = currentItem.maxStackSize - currentCount;
            int amountToAdd = Mathf.Min(spaceLeft, droppedItemScript.count);

            if (amountToAdd > 0)
            {
                this.currentCount += amountToAdd;

                InventorySlot oldInvSlot = oldSlotTransform.GetComponent<InventorySlot>();
                CraftingSlot oldCraftSlot = oldSlotTransform.GetComponent<CraftingSlot>();

                if (oldInvSlot != null && !droppedItemScript.isSplitDrag)
                {
                    oldInvSlot.currentCount -= amountToAdd;
                    if (oldInvSlot.currentCount <= 0 && !oldInvSlot.lockContent) oldInvSlot.ClearSlot();
                    else oldInvSlot.UpdateUI();
                }
                else if (oldCraftSlot != null && !droppedItemScript.isSplitDrag)
                {
                    oldCraftSlot.currentCount -= amountToAdd;
                    if (oldCraftSlot.currentCount <= 0)
                    {
                        oldCraftSlot.currentItem = null;
                        oldCraftSlot.currentCount = 0;
                        oldCraftSlot.iconDisplay = null;
                    }
                    
                    // UI refresh for old inventory slot
                    DraggableItem oldItemScript = oldCraftSlot.GetComponentInChildren<DraggableItem>();
                    if (oldItemScript != null) oldItemScript.RefreshCount(oldCraftSlot.currentCount);
                }

                // UI refresh for current slot
                DraggableItem residentScript = GetComponentInChildren<DraggableItem>();
                if (residentScript != null)
                {
                    residentScript.RefreshCount(this.currentCount);
                }

                Destroy(droppedObj);

                if (CraftingUI.Instance != null) CraftingUI.Instance.UpdateCraftingGrid();
                TooltipManager.Instance.HideTooltip();
                return;
            }
        }

        if (transform.childCount == 0)
        {
            droppedItemScript.parentAfterDrag = transform;

            this.currentItem = droppedItemScript.itemData;
            this.currentCount = droppedItemScript.count; 
            this.iconDisplay = droppedObj.GetComponent<Image>();

            if (!droppedItemScript.isSplitDrag)
            {
                UpdateOldSlot(oldSlotTransform, null, 0, null);
            }
        }
        else
        {
            // Block for a case when you try to swap items while moving one item from a pile
            if (droppedItemScript.isSplitDrag) return;

            GameObject residentObj = transform.GetChild(0).gameObject;
            DraggableItem residentItemScript = residentObj.GetComponent<DraggableItem>();

            if (residentItemScript != null)
            {
                int residentCount = residentItemScript.count;
                ItemData residentData = residentItemScript.itemData;
                Image residentIcon = residentObj.GetComponent<Image>();

                residentItemScript.transform.SetParent(oldSlotTransform);
                residentItemScript.parentAfterDrag = oldSlotTransform;
                residentItemScript.transform.localPosition = Vector3.zero;
                residentItemScript.transform.localScale = Vector3.one;

                droppedItemScript.parentAfterDrag = transform;

                this.currentItem = droppedItemScript.itemData;
                this.currentCount = droppedItemScript.count;
                this.iconDisplay = droppedObj.GetComponent<Image>();

                UpdateOldSlot(oldSlotTransform, residentData, residentCount, residentIcon);
            }
        }
        
        if (CraftingUI.Instance != null)
        {
            CraftingUI.Instance.UpdateCraftingGrid();
        }
        
        TooltipManager.Instance.HideTooltip();
    }

    private void UpdateOldSlot(Transform oldSlotTransform, ItemData newItem, int newCount, Image newIcon)
    {
        InventorySlot invSlot = oldSlotTransform.GetComponent<InventorySlot>();
        if (invSlot != null)
        {
            invSlot.currentItem = newItem;
            invSlot.currentCount = newCount;
            invSlot.iconDisplay = newIcon;
            invSlot.UpdateUI();
            return; 
        }

        CraftingSlot craftSlot = oldSlotTransform.GetComponent<CraftingSlot>();
        if (craftSlot != null)
        {
            craftSlot.currentItem = newItem;
            craftSlot.currentCount = newCount;
            craftSlot.iconDisplay = newIcon;
            
            DraggableItem craftItem = craftSlot.GetComponentInChildren<DraggableItem>();
            if (craftItem != null && newCount > 0) craftItem.RefreshCount(newCount);
            
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