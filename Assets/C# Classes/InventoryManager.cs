using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Singleton - pozwala odwołać się do managera przez InventoryManager.instance
    public static InventoryManager instance;

    [Header("Konfiguracja")]
    // Przeciągnij tu wszystkie swoje sloty z Hierarchy (InventoryPanel -> Slot)
    public InventorySlot[] inventorySlots;

    private void Awake()
    {
        // Konfiguracja Singletona
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Główna metoda dodawania przedmiotu
    public bool AddItemToFirstFreeSlot(ItemData item, Sprite icon)
    {
        // Sprawdzamy każdy slot po kolei
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            // Zakładam, że w InventorySlot.cs masz zmienną 'currentItem'
            // Jeśli slot jest pusty (currentItem == null):
            if (slot.currentItem == null)
            {
                // 1. Zapisz dane w slocie (LOGIKA)
                slot.currentItem = item;

                // 2. Ustaw wygląd (GRAFIKA)
                // Zakładam, że w InventorySlot.cs masz Image 'iconDisplay'
                slot.iconDisplay.sprite = icon; // lub item.icon
                slot.iconDisplay.enabled = true;

                // ============================================================
                // 3. KLUCZOWA POPRAWKA DLA CRAFTINGU I PRZECIĄGANIA
                // ============================================================
                // Musimy znaleźć skrypt DraggableItem na obrazku i dać mu dane
                DraggableItem draggable = slot.iconDisplay.GetComponent<DraggableItem>();

                if (draggable != null)
                {
                    draggable.itemData = item; // <-- To naprawia błąd "Na stole jest [PUSTO]"
                    
                    // Resetujemy pozycję ikonki na środek slota (na wypadek gdyby była przesunięta)
                    draggable.transform.localPosition = Vector3.zero;
                }
                else
                {
                    Debug.LogWarning($"Slot {slot.name} nie ma komponentu DraggableItem na obrazku!");
                }
                // ============================================================

                Debug.Log($"Dodano przedmiot: {item.itemName} do slota nr {i}");
                return true; // Sukces, przerywamy pętlę
            }
        }

        Debug.Log("Ekwipunek jest pełny!");
        return false; // Brak miejsca
    }
}