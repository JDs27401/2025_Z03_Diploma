using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [Header("Główne Referencje")]
    public CraftingManager craftingManager; // Logika sprawdzania receptur
    
    // Siatka 2x2 - sloty muszą być przypisane w kolejności:
    // 0: Lewy-Góra, 1: Prawy-Góra, 2: Lewy-Dół, 3: Prawy-Dół
    public CraftingSlot[] gridSlots; 

    [Header("Ustawienia Wyniku (Result Slot)")]
    public Image resultSlotImage;        // "Duch" - półprzezroczysty podgląd wyniku
    public Transform resultSlotContainer; // Fizyczny obiekt ResultSlot (rodzic dla nowego przedmiotu)
    public Button craftButton;           // Przycisk "Wytwórz"
    
    [Header("Prefab")]
    // Tutaj przeciągnij ten sam prefab, którego używa InventoryManager (Uniwersalny UI Item)
    public GameObject draggableItemPrefab; 

    // Przechowuje dane przedmiotu, który powstanie z aktualnego ułożenia
    private ItemData currentResultItem;

    private void Start()
    {
        // Na starcie ukrywamy podgląd i blokujemy przycisk
        resultSlotImage.gameObject.SetActive(false);
        if(craftButton) craftButton.interactable = false;
    }

    // Ta metoda jest wołana przez sloty (CraftingSlot), gdy cokolwiek się zmieni
    public void UpdateCraftingGrid()
    {
        // --- TARCZA OCHRONNA ---
        // 1. Sprawdzamy, czy w slocie wynikowym leży już wyprodukowany przedmiot.
        // Jeśli tak, przerywamy sprawdzanie! Nie chcemy, żeby zmiana składników usunęła gotowy przedmiot.
        if (resultSlotContainer.GetComponentInChildren<DraggableItem>() != null)
        {
            // Ukrywamy "ducha" (bo mamy prawdziwy przedmiot) i blokujemy przycisk
            resultSlotImage.gameObject.SetActive(false);
            if(craftButton) craftButton.interactable = false;
            return; 
        }

        // 2. Zbieramy dane ze wszystkich 4 slotów
        ItemData[] currentItems = new ItemData[4];

        for (int i = 0; i < gridSlots.Length; i++)
        {
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

        // 3. Pytamy Managera czy to pasuje do jakiegoś przepisu
        currentResultItem = craftingManager.CheckForRecipe(currentItems);

        // 4. Aktualizujemy wygląd (pokazujemy lub ukrywamy "ducha")
        UpdateResultUI();
    }

    private void UpdateResultUI()
    {
        if (currentResultItem != null)
        {
            // Mamy przepis -> Pokaż "Ducha"
            resultSlotImage.gameObject.SetActive(true);
            resultSlotImage.sprite = currentResultItem.icon;
            resultSlotImage.preserveAspect = true; // Zachowaj proporcje obrazka
            
            if(craftButton) craftButton.interactable = true;
        }
        else
        {
            // Brak przepisu -> Ukryj wszystko
            resultSlotImage.gameObject.SetActive(false);
            if(craftButton) craftButton.interactable = false;
        }
    }
    
    // Metoda podpięta pod przycisk "Wytwórz"
    public void OnCraftButtonPress()
    {
        if (currentResultItem == null) return;
        
        // Zabezpieczenie: czy slot wynikowy na pewno jest pusty?
        if (resultSlotContainer.GetComponentInChildren<DraggableItem>() != null) return;

        // =================================================================
        // KROK 1: NAJPIERW TWORZYMY NOWY PRZEDMIOT
        // =================================================================
        // Tworzymy go zanim zniszczymy składniki, dzięki temu "Tarcza Ochronna"
        // w UpdateCraftingGrid zadziała i nie wyczyści nam wyniku.
        
        GameObject newItem = Instantiate(draggableItemPrefab, resultSlotContainer);
        
        DraggableItem draggable = newItem.GetComponent<DraggableItem>();
        Image itemImage = newItem.GetComponent<Image>();

        if (draggable != null && itemImage != null)
        {
            // Wypełniamy uniwersalny prefab danymi z przepisu
            draggable.itemData = currentResultItem; 
            itemImage.sprite = currentResultItem.icon; 
            itemImage.preserveAspect = true;
            
            // Resetujemy pozycję i skalę, żeby ładnie leżał w slocie
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localScale = Vector3.one;
        }

        // =================================================================
        // KROK 2: NISZCZYMY SKŁADNIKI
        // =================================================================
        foreach (var slot in gridSlots)
        {
            DraggableItem item = slot.GetComponentInChildren<DraggableItem>();
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        // =================================================================
        // KROK 3: SPRZĄTANIE UI
        // =================================================================
        resultSlotImage.gameObject.SetActive(false); // Ukrywamy "ducha"
        if(craftButton) craftButton.interactable = false; // Blokujemy przycisk, bo wynik jest zajęty
        
        // Czyścimy pamięć o przepisie (przedmiot jest już fizyczny, przepis niepotrzebny)
        currentResultItem = null;
    }
}