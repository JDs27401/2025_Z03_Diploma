using UnityEngine;
using C__Classes.Systems;
using C__Classes.Managers; // Dodajemy dostęp do InventoryManager

public class LockerUIManager : MonoBehaviour
{
    public static LockerUIManager Instance { get; private set; }

    [Header("Elementy UI")]
    public GameObject lockerUIPanel;
    
    public InventorySlot[] lockerSlots; 

    private LockerInteractable currentLocker;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lockerUIPanel.SetActive(false);
    }

    public void OpenLockerUI(LockerInteractable locker)
    {
        currentLocker = locker;
        lockerUIPanel.SetActive(true);
        RefreshUI();
    }

    public void CloseUI()
    {
        if (currentLocker != null)
        {
            for (int i = 0; i < 2; i++)
            {
                if (lockerSlots[i].currentItem == null && currentLocker.slotItems[i] != null)
                {
                    string itemID = currentLocker.GetSlotID(i);
                    if (LootManager.Instance != null) LootManager.Instance.MarkAsLooted(itemID);
                    
                    currentLocker.slotItems[i] = null;
                }
            }
            
            for (int i = 0; i < 2; i++)
            {
                lockerSlots[i].ClearSlot();
                foreach (Transform child in lockerSlots[i].transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        lockerUIPanel.SetActive(false);
        currentLocker = null;
    }

    private void RefreshUI()
    {
        for (int i = 0; i < 2; i++)
        {
            lockerSlots[i].ClearSlot();
            foreach (Transform child in lockerSlots[i].transform)
            {
                Destroy(child.gameObject);
            }

            GameObject itemPrefab = currentLocker.slotItems[i];

            if (itemPrefab != null)
            {
                PickableItem pickable = itemPrefab.GetComponent<PickableItem>();
                if (pickable != null && pickable.itemData != null)
                {
                    GameObject newItemGo = Instantiate(InventoryManager.Instance.inventoryItemPrefab, lockerSlots[i].transform);
                    
                    RectTransform rect = newItemGo.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.localScale = Vector3.one;
                        rect.anchorMin = Vector2.zero;
                        rect.anchorMax = Vector2.one;
                        rect.anchoredPosition = Vector2.zero;
                        rect.offsetMin = Vector2.zero;
                        rect.offsetMax = Vector2.zero;
                    }

                    DraggableItem dragItem = newItemGo.GetComponent<DraggableItem>();
                    if (dragItem != null)
                    {
                        dragItem.itemData = pickable.itemData;
                        dragItem.count = pickable.amount > 0 ? pickable.amount : 1;
                        dragItem.weaponInstanceState = pickable.droppedWeaponState != null ? pickable.droppedWeaponState.Clone() : null;
                        dragItem.EnsureWeaponStateInitialized();
                    }

                    UnityEngine.UI.Image image = newItemGo.GetComponent<UnityEngine.UI.Image>();
                    if (image == null) image = newItemGo.GetComponentInChildren<UnityEngine.UI.Image>();

                    if (image != null && pickable.itemData.icon != null)
                    {
                        image.sprite = pickable.itemData.icon;
                        image.color = Color.white;
                        image.enabled = true;
                    }
                    
                    lockerSlots[i].currentItem = pickable.itemData;
                    lockerSlots[i].currentCount = dragItem.count;
                    lockerSlots[i].iconDisplay = image;
                    lockerSlots[i].UpdateUI();
                }
            }
        }
    }
}