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
                            Debug.Log($"[Inventory] Dodano do stosu: {item.itemName}");
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
}