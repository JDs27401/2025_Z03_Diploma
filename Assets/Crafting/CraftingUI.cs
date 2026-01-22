using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    [Header("Główne Referencje")]
    public CraftingManager craftingManager; 
    public CraftingSlot[] gridSlots; 

    [Header("Ustawienia Wyniku")]
    public Image resultSlotImage; 
    public Transform resultSlotContainer; 
    public Button craftButton; 
    
    [Header("Prefab")]
    public GameObject draggableItemPrefab; 

    private ItemData currentResultItem;

    private void Start()
    {
        if (resultSlotImage != null) resultSlotImage.gameObject.SetActive(false);
        if (craftButton != null) craftButton.interactable = false;
    }

    public void UpdateCraftingGrid()
    {
        if (resultSlotContainer.childCount > 0)
        {
            if (resultSlotContainer.GetComponentInChildren<DraggableItem>() != null)
            {
                resultSlotImage.gameObject.SetActive(false);
                if(craftButton) craftButton.interactable = false;
                return; 
            }
        }

        ItemData[] currentItems = new ItemData[4];

        for (int i = 0; i < gridSlots.Length; i++)
        {
            if (gridSlots[i].currentItem != null)
            {
                currentItems[i] = gridSlots[i].currentItem;
            }
            else
            {
                currentItems[i] = null;
            }
        }

        if (craftingManager != null)
        {
            currentResultItem = craftingManager.CheckForRecipe(currentItems);
            UpdateResultUI();
        }
    }

    private void UpdateResultUI()
    {
        if (currentResultItem != null)
        {
            if (resultSlotImage != null)
            {
                resultSlotImage.gameObject.SetActive(true);
                resultSlotImage.sprite = currentResultItem.icon;
                resultSlotImage.color = new Color(1, 1, 1, 0.5f); 
            }
            
            if(craftButton != null) craftButton.interactable = true;
        }
        else
        {
            if (resultSlotImage != null) resultSlotImage.gameObject.SetActive(false);
            if (craftButton != null) craftButton.interactable = false;
        }
    }
    
    public void OnCraftButtonPress()
    {
        if (currentResultItem == null) return;
        if (resultSlotContainer.GetComponentInChildren<DraggableItem>() != null) return;

        GameObject newItem = Instantiate(draggableItemPrefab, resultSlotContainer);
        DraggableItem draggable = newItem.GetComponent<DraggableItem>();
        Image itemImage = newItem.GetComponent<Image>();

        if (draggable != null && itemImage != null)
        {
            draggable.itemData = currentResultItem; 
            draggable.count = 1;
            
            draggable.RefreshCount(1);

            itemImage.sprite = currentResultItem.icon; 
            itemImage.color = Color.white;
            itemImage.raycastTarget = true; 
            
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localScale = Vector3.one;
            
            RectTransform rect = newItem.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        }

        foreach (var slot in gridSlots)
        {
            if (slot.currentItem != null)
            {
                slot.currentCount--;

                DraggableItem itemScript = slot.GetComponentInChildren<DraggableItem>();

                if (slot.currentCount <= 0)
                {
                    slot.currentItem = null;
                    slot.iconDisplay = null;
                    slot.currentCount = 0;

                    if (itemScript != null) Destroy(itemScript.gameObject);
                }
                else
                {
                    if (itemScript != null)
                    {
                        itemScript.RefreshCount(slot.currentCount);
                    }
                }
            }
        }

        resultSlotImage.gameObject.SetActive(false); 
        if(craftButton) craftButton.interactable = false; 
        currentResultItem = null;
        
        UpdateCraftingGrid();
    }
}