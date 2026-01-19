using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Konfiguracja UI")]
    public InventorySlot[] slots; 

    [Header("Wymagane Prefaby")]
    public GameObject inventoryItemPrefab; 

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        // Czyszczenie startowe
        if (slots != null)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot != null)
                {
                    slot.currentItem = null; 
                    foreach (Transform child in slot.transform) DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    public bool AddItemToFirstFreeSlot(ItemData itemData, Sprite icon)
    {
        // 0. ZABEZPIECZENIE: Nie dodawaj pustych danych
        if (itemData == null)
        {
            Debug.LogError("Próbujesz dodać przedmiot bez ItemData! Sprawdź PickableItem w inspektorze.");
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            // 1. SPRAWDZENIE DESYNCHRONIZACJI (Autonaprawa)
            // Jeśli system myśli, że slot jest pusty (currentItem == null),
            // ale fizycznie widać tam przedmiot (ma DraggableItem), to znaczy, że coś się zepsuło.
            if (slot.currentItem == null && slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                // Sprawdzamy czy to prawdziwy przedmiot (ma skrypt), czy tylko śmieć/placeholder
                DraggableItem itemInSlot = child.GetComponent<DraggableItem>();
                
                if (itemInSlot != null && child.gameObject.activeSelf)
                {
                    // Slot jest zajęty, tylko dane zniknęły! Naprawiamy to.
                    // Przypisujemy dane z przedmiotu do slotu.
                    if (itemInSlot.itemData != null)
                    {
                        slot.currentItem = itemInSlot.itemData;
                        Debug.LogWarning($"Naprawiono slot {i}: Był wizualnie zajęty, ale logicznie pusty.");
                    }
                    else
                    {
                        // Jeśli nawet przedmiot nie ma danych, to traktujemy go jako zajęty
                        // (żeby go nie nadpisać), ale wypisujemy błąd.
                        Debug.LogError($"Slot {i} zawiera przedmiot bez danych! Pomijam go.");
                        continue; // Idź do następnego slotu
                    }
                }
            }

            // 2. WŁAŚCIWE DODAWANIE (Tylko jeśli slot jest naprawdę pusty)
            if (slot.currentItem == null)
            {
                // Czyścimy stare śmieci (ale tylko te, które nie są DraggableItem - np. placeholdery)
                for (int k = slot.transform.childCount - 1; k >= 0; k--)
                {
                    GameObject child = slot.transform.GetChild(k).gameObject;
                    child.SetActive(false); 
                    child.transform.SetParent(null); 
                    Destroy(child); 
                }

                // Tworzymy nowy przedmiot
                GameObject newItemObj = Instantiate(inventoryItemPrefab, slot.transform);
                newItemObj.transform.localPosition = Vector3.zero;
                newItemObj.transform.localScale = Vector3.one;

                Image imageComponent = newItemObj.GetComponent<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = icon;
                    imageComponent.color = Color.white;
                    imageComponent.enabled = true;
                }

                DraggableItem draggable = newItemObj.GetComponent<DraggableItem>();
                if (draggable != null)
                {
                    draggable.itemData = itemData; 
                    draggable.parentAfterDrag = slot.transform;
                }

                slot.currentItem = itemData;
                slot.iconDisplay = imageComponent; 

                Debug.Log($"[SUKCES] Dodano {itemData.itemName} do slotu {i}");
                return true;
            }
        }
        
        Debug.Log("Ekwipunek pełny!");
        return false;
    }
}