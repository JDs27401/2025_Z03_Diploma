using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [Header("References")]
    public CraftingManager craftingManager; // Przypisz Manager z poprzedniego etapu
    
    // Siatka 2x2 - MUSZĄ BYĆ PRZYPISANE W KOLEJNOŚCI:
    // 0: Lewy-Góra, 1: Prawy-Góra
    // 2: Lewy-Dół, 3: Prawy-Dół
    public CraftingSlot[] gridSlots; 

    // Slot wynikowy (gdzie pojawi się wytworzony przedmiot)
    public Image resultSlotImage;
    public Button craftButton; // Opcjonalnie przycisk "Wytwórz"

    private ItemData currentResultItem;

    private void Start()
    {
        // Na starcie czyścimy wynik
        resultSlotImage.gameObject.SetActive(false);
        if(craftButton) craftButton.interactable = false;
    }

    // Ta metoda jest wołana przez sloty, gdy gracz coś położy
    public void UpdateCraftingGrid()
    {
        ItemData[] currentItems = new ItemData[4];

        for (int i = 0; i < gridSlots.Length; i++)
        {
            // Sprawdzamy, czy w slocie jest dziecko (DraggableItem)
            DraggableItem itemInSlot = gridSlots[i].GetComponentInChildren<DraggableItem>();
            
            if (itemInSlot != null)
            {
                currentItems[i] = itemInSlot.itemData;
            }
            else
            {
                currentItems[i] = null;
            }
        }

        // Pytamy CraftingManager o wynik
        currentResultItem = craftingManager.CheckForRecipe(currentItems);

        UpdateResultUI();
    }

    private void UpdateResultUI()
{
    if (currentResultItem != null)
    {
        resultSlotImage.gameObject.SetActive(true);
        resultSlotImage.sprite = currentResultItem.icon;

        // 1. Najpierw resetujemy skalę (czasem się psuje przy zmianach rodzica)
        resultSlotImage.rectTransform.localScale = Vector3.one;

        // 2. WYMUSZAMY ROZMIAR (Zmień 100, 100 na wielkość swojego slota)
        resultSlotImage.rectTransform.sizeDelta = new Vector2(100, 100);

        // 3. Zachowujemy proporcje (żeby miecz nie był "gruby", tylko zmieścił się w ramce)
        resultSlotImage.preserveAspect = true;

        if(craftButton) craftButton.interactable = true;
    }
    else
    {
        resultSlotImage.gameObject.SetActive(false);
        if(craftButton) craftButton.interactable = false;
    }
}
    
    // Metoda dla przycisku "Wytwórz"
    public void OnCraftButtonPress()
    {
        if (currentResultItem == null) return;

        Debug.Log("Wytworzono przedmiot: " + currentResultItem.itemName);
        
        // TUTAJ: Dodaj kod dodawania przedmiotu do ekwipunku gracza
        
        // Czyścimy siatkę (niszczymy zużyte przedmioty)
        foreach (var slot in gridSlots)
        {
            DraggableItem item = slot.GetComponentInChildren<DraggableItem>();
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        
        UpdateCraftingGrid(); // Odśwież stan (wynik zniknie)
    }
}