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
        // Tarcza ochronna: jeśli wynik jest zajęty, nie pozwól na zmiany
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
            // Pobieramy dane bezpośrednio ze slotu (najszybsza metoda)
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

        // 1. Tworzenie przedmiotu
        GameObject newItem = Instantiate(draggableItemPrefab, resultSlotContainer);
        DraggableItem draggable = newItem.GetComponent<DraggableItem>();
        Image itemImage = newItem.GetComponent<Image>();

        if (draggable != null && itemImage != null)
        {
            draggable.itemData = currentResultItem; 
            itemImage.sprite = currentResultItem.icon; 
            itemImage.color = Color.white;
            itemImage.raycastTarget = true; 
            
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localScale = Vector3.one;
        }

        // 2. Bezpieczne niszczenie składników (FIX MissingReferenceException)
        foreach (var slot in gridSlots)
        {
            // Najpierw czyścimy referencje logiczne w slocie
            slot.currentItem = null; 
            slot.iconDisplay = null;

            // Teraz niszczymy fizyczne obiekty
            foreach(Transform child in slot.transform)
            {
                if (child != null && child.gameObject != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // 3. Sprzątanie UI
        resultSlotImage.gameObject.SetActive(false); 
        if(craftButton) craftButton.interactable = false; 
        currentResultItem = null;
    }
}