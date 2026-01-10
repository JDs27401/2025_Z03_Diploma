using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class InventorySlot : MonoBehaviour, IDropHandler
{
    // --- ZMIENNE PUBLICZNE (Wymagane przez InventoryManager) ---
    // Przywracamy je, aby InventoryManager przestał wyrzucać błędy.
    // UWAGA: Jeśli Twoja klasa z danymi przedmiotu nie nazywa się "ItemData", 
    // zmień typ zmiennej poniżej na właściwy (np. InventoryItemData, Item itp.)
    public ItemData currentItem; 
    public Image iconDisplay;

    // --- LOGIKA UPUSZCZANIA ---
    public void OnDrop(PointerEventData eventData)
    {
        // Sprawdź, czy coś jest przeciągane
        if (eventData.pointerDrag == null) return;

        // 1. CZYSZCZENIE "DUCHÓW" (Fix na błąd po craftingu)
        // Sprawdzamy, czy w slocie jest obiekt, który stracił ikonę/obrazek
        if (transform.childCount > 0)
        {
            GameObject currentObject = transform.GetChild(0).gameObject;
            Image objectImage = currentObject.GetComponent<Image>();

            bool isGhost = false;

            if (objectImage == null) isGhost = true;
            else if (objectImage.sprite == null || objectImage.enabled == false) isGhost = true;

            if (isGhost)
            {
                // Usuwamy "niewidzialny" obiekt natychmiast
                DestroyImmediate(currentObject);
            }
        }

        // 2. WŁAŚCIWE WKŁADANIE PRZEDMIOTU
        if (transform.childCount == 0)
        {
            // TUTAJ ZMIANA: Używamy DraggableItem zgodnie z Twoją nazwą klasy
            DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            
            if (draggableItem != null)
            {
                draggableItem.parentAfterDrag = transform;
            }
        }
    }
}