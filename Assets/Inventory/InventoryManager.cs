using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Konfiguracja")]
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void InitializeStaticSlot(InventorySlot slot, ItemData item)
    {
        SpawnNewItemInSlot(slot, item, 0);

        if (slot.iconDisplay != null)
        {
            slot.iconDisplay.raycastTarget = false; 
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item.isStackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.currentItem == item)
                {
                    if (slot.currentCount < item.maxStackSize)
                    {
                        int spaceInStack = item.maxStackSize - slot.currentCount;
                        int amountToAdd = Mathf.Min(spaceInStack, amount);

                        slot.currentCount += amountToAdd;
                        
                        if (slot.iconDisplay != null) slot.iconDisplay.color = Color.white;

                        slot.UpdateUI();

                        amount -= amountToAdd;
                        
                        if (amount <= 0) 
                        {
                            return true; 
                        }
                    }
                }
            }
        }

        while (amount > 0)
        {
            InventorySlot emptySlot = FindFirstFreeSlot(item.itemType);

            if (emptySlot != null)
            {
                int amountToAdd = item.isStackable ? Mathf.Min(item.maxStackSize, amount) : 1;
                
                SpawnNewItemInSlot(emptySlot, item, amountToAdd);
                
                amount -= amountToAdd;
            }
            else
            {
                Debug.Log("[Inventory] Brak miejsca w ekwipunku.");
                return false; 
            }
        }

        return true;
    }

    private void SpawnNewItemInSlot(InventorySlot slot, ItemData item, int amount)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        
        RectTransform rect = newItemGo.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero; 
            rect.anchorMax = Vector2.one;  

            rect.offsetMin = Vector2.zero; 
            rect.offsetMax = Vector2.zero; 
        }

        DraggableItem dragItem = newItemGo.GetComponent<DraggableItem>();
        if (dragItem != null)
        {
            dragItem.itemData = item;
            dragItem.count = amount;
        }

        Image image = newItemGo.GetComponent<Image>();
        if (image == null) image = newItemGo.GetComponentInChildren<Image>();

        if (image != null)
        {
            if (item.icon != null)
            {
                image.sprite = item.icon;
                image.color = Color.white; 
                image.enabled = true;
            }
        }

        slot.currentItem = item;
        slot.currentCount = amount;
        slot.iconDisplay = image;
        
        slot.UpdateUI();
    }

    public InventorySlot FindFirstFreeSlot(ItemType typeToCheck)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.currentItem == null)
            {
                if (slot.allowedType == typeToCheck && slot.allowedType != ItemType.General)
                {
                    return slot;
                }
            }
        }

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.currentItem == null)
            {
                if (slot.allowedType == ItemType.General)
                {
                    return slot;
                }
            }
        }

        return null;
    }

    public void DropItem(DraggableItem draggableItem)
    {
        if (draggableItem == null || draggableItem.itemData == null || draggableItem.itemData.dropPrefab == null)
        {
            Debug.LogWarning("[Inventory] Przedmiot nie ma przypisanego poprawnego dropPrefab!");
            return;
        }

        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogWarning("[Inventory] Nie znaleziono gracza na scenie!");
            return;
        }

        Vector3 dropOffset = new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(-0.4f, 0.4f), 0);
        Vector3 dropPosition = player.transform.position + dropOffset;
        dropPosition.z = player.transform.position.z; 

        GameObject droppedObj = Instantiate(draggableItem.itemData.dropPrefab);
        droppedObj.transform.position = dropPosition;
        droppedObj.transform.rotation = Quaternion.identity;
        droppedObj.name = draggableItem.itemData.itemName + " (Dropped)";
        
        C__Classes.Systems.LootTracker lootTracker = droppedObj.GetComponent<C__Classes.Systems.LootTracker>();
        if (lootTracker != null)
        {
            lootTracker.isDynamicDrop = true;
        }

        droppedObj.SetActive(true);

        PickableItem pickable = droppedObj.GetComponent<PickableItem>();
        if (pickable != null)
        {
            pickable.amount = 1;
            pickable.itemData = draggableItem.itemData;
        }

        Transform targetParent = draggableItem.parentAfterDrag != null ? draggableItem.parentAfterDrag : draggableItem.transform.parent;
        InventorySlot slot = targetParent.GetComponent<InventorySlot>();
        
        draggableItem.count--;

        if (slot != null)
        {
            slot.currentCount = draggableItem.count;
        }

        if (draggableItem.count <= 0)
        {
            if (TooltipManager.instance != null) TooltipManager.instance.HideTooltip();
            
            if (slot != null) slot.ClearSlot();
            Destroy(draggableItem.gameObject);
        }
        else
        {
            draggableItem.RefreshCount(draggableItem.count);
            if (slot != null) slot.UpdateUI();
        }

        // Aktualizacja hałasu gracza po wyrzuceniu przedmiotu (zmiana wagi)
        if (C__Classes.Systems.PlayerNoiseSystem.Instance != null)
        {
            C__Classes.Systems.PlayerNoiseSystem.Instance.UpdateNoiseRadius();
        }
    }

    public float GetTotalWeight()
    {
        float totalWeight = 0f;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.currentItem != null)
            {
                totalWeight += slot.currentItem.weight * slot.currentCount;
            }
        }
        return totalWeight;
    }
}