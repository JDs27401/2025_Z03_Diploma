using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Configuration UI")]
    public TextMeshProUGUI quantityText;

    [Header("Logic Settings")]
    public bool isReadOnly = false;
    public ItemType allowedType = ItemType.General;
    public bool lockContent = false;
    
    [Header("Static slot (Optional)")]
    public ItemData dedicatedItem;

    [Header("Slot state")]
    public ItemData currentItem;
    public int currentCount = 0;
    public Image iconDisplay;
    
    [Header("Selection UI")]
    public Image highlightOutline;

    private void Start()
    {
        if (dedicatedItem != null && InventoryManager.instance != null)
        {
            InventoryManager.instance.InitializeStaticSlot(this, dedicatedItem);
        }
        UpdateUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isReadOnly) return;
        if (eventData.pointerDrag == null) return;

        GameObject droppedObj = eventData.pointerDrag;
        DraggableItem droppedItemScript = droppedObj.GetComponent<DraggableItem>();
        if (droppedItemScript == null) return;

        if (allowedType != ItemType.General)
        {
            if (droppedItemScript.itemData.itemType != allowedType) return;
        }

        Transform oldSlotTransform = droppedItemScript.parentAfterDrag;
        
        InventorySlot oldSlot = oldSlotTransform.GetComponent<InventorySlot>();
        CraftingSlot oldCraftingSlot = oldSlotTransform.GetComponent<CraftingSlot>(); 
        
        if (currentItem != null && currentItem == droppedItemScript.itemData && currentItem.isStackable)
        {
            if (oldSlot == this) return;

            int spaceLeft = currentItem.maxStackSize - currentCount;
            int amountToAdd = Mathf.Min(spaceLeft, droppedItemScript.count);

            if (amountToAdd > 0)
            {
                this.currentCount += amountToAdd;
                
                if (oldSlot != null && oldSlot != this && !droppedItemScript.isSplitDrag)
                {
                    oldSlot.currentCount -= amountToAdd;
                    if (oldSlot.currentCount <= 0 && !oldSlot.lockContent)
                    {
                        oldSlot.ClearSlot();
                    }
                    else
                    {
                        oldSlot.UpdateUI();
                    }
                }
                else if (oldCraftingSlot != null && !droppedItemScript.isSplitDrag)
                {
                    oldCraftingSlot.currentCount -= amountToAdd;
                    if (oldCraftingSlot.currentCount <= 0)
                    {
                        oldCraftingSlot.currentItem = null;
                        oldCraftingSlot.currentCount = 0;
                        oldCraftingSlot.iconDisplay = null;
                    }
                }
                
                Destroy(droppedObj);
                UpdateUI();
                
                if (CraftingUI.instance != null) CraftingUI.instance.UpdateCraftingGrid();
                return;
            }
        }

        if (lockContent && oldSlot != this) return;

        if (currentItem == null || oldSlot == this)
        {
            droppedItemScript.parentAfterDrag = transform;
            
            this.currentItem = droppedItemScript.itemData;
            this.currentCount = droppedItemScript.count;
            this.iconDisplay = droppedObj.GetComponent<Image>();

            if (oldSlot != null && oldSlot != this && !droppedItemScript.isSplitDrag)
            {
                oldSlot.ClearSlot();
            }
            else if (oldCraftingSlot != null && !droppedItemScript.isSplitDrag)
            {
                oldCraftingSlot.currentItem = null;
                oldCraftingSlot.currentCount = 0;
                oldCraftingSlot.iconDisplay = null;
            }

            UpdateUI();
        }
        // Swap scenario
        else
        {
            if (transform.childCount == 0) return;
            
            if (droppedItemScript.isSplitDrag) return; 

            GameObject residentObj = transform.GetChild(0).gameObject;
            DraggableItem residentScript = residentObj.GetComponent<DraggableItem>();
            
            if (oldSlot != null && oldSlot.allowedType != ItemType.General)
            {
                 if (currentItem.itemType != oldSlot.allowedType) return;
            }

            residentScript.transform.SetParent(oldSlotTransform);
            residentScript.parentAfterDrag = oldSlotTransform;
            
            RectTransform resRect = residentScript.GetComponent<RectTransform>();
            if(resRect) {
                resRect.anchoredPosition = Vector2.zero; 
                resRect.localScale = Vector3.one;
                resRect.offsetMin = Vector2.zero;
                resRect.offsetMax = Vector2.zero;
            }

            droppedItemScript.parentAfterDrag = transform;

            ItemData tempItem = this.currentItem;
            int tempCount = this.currentCount;

            this.currentItem = droppedItemScript.itemData;
            this.currentCount = droppedItemScript.count;
            this.iconDisplay = droppedObj.GetComponent<Image>();

            if (oldSlot != null)
            {
                oldSlot.currentItem = tempItem;
                oldSlot.currentCount = tempCount;
                oldSlot.iconDisplay = residentObj.GetComponent<Image>();
                oldSlot.UpdateUI();
            }
            else if (oldCraftingSlot != null)
            {
                oldCraftingSlot.currentItem = tempItem;
                oldCraftingSlot.currentCount = tempCount;
                oldCraftingSlot.iconDisplay = residentObj.GetComponent<Image>();
            }

            UpdateUI();
        }
        
        if (CraftingUI.instance != null) CraftingUI.instance.UpdateCraftingGrid();
    }

    public void UpdateUI()
    {
        DraggableItem itemScript = GetComponentInChildren<DraggableItem>();

        if (itemScript != null)
        {
            itemScript.count = currentCount;
            itemScript.UpdateTextPosition();

            TextMeshProUGUI textOnItem = itemScript.amountText;
            if (textOnItem != null)
            {
                bool showText = false;
                if (currentItem != null)
                {
                    if (currentItem.itemType != ItemType.General) showText = true;
                    else if (currentCount > 1) showText = true;
                }

                textOnItem.text = currentCount.ToString();
                textOnItem.gameObject.SetActive(showText);
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentCount = 0;
        iconDisplay = null;
        UpdateUI();
    }
    
    public void SetSelected(bool isSelected)
    {
        if (highlightOutline != null)
        {
            highlightOutline.enabled = isSelected;
        }
    }
}