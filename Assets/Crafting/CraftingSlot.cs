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

        if (transform.childCount == 0)
        {
            droppedItemScript.parentAfterDrag = transform;

            this.currentItem = droppedItemScript.itemData;
            this.currentCount = droppedItemScript.count; 
            this.iconDisplay = droppedObj.GetComponent<Image>();

            UpdateOldSlot(oldSlotTransform, null, 0, null);
        }
        else
        {
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
        
        if (CraftingUI.instance != null)
        {
            CraftingUI.instance.UpdateCraftingGrid();
        }
        
        TooltipManager.instance.HideTooltip();
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