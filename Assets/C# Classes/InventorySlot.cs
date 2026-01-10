using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // <--- WAŻNE: To jest potrzebne

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public ItemData currentItem;
    public Image iconDisplay;

    private void Awake()
    {
        // Upewniamy się, że na starcie slot jest czysty
        if (currentItem == null) ClearSlot();
    }

    public void ClearSlot()
    {
        currentItem = null;
        if (iconDisplay != null)
        {
            iconDisplay.sprite = null;
            iconDisplay.enabled = false;
        }
    }

    // --- TO JEST NOWA CZĘŚĆ ODPOWIEDZIALNA ZA PRZYJMOWANIE PRZEDMIOTÓW ---
    public void OnDrop(PointerEventData eventData)
    {
        // Sprawdzamy, czy slot jest pusty (żeby nie nałożyć przedmiotu na przedmiot)
        // Jeśli chcesz pozwalać na zamianę miejscami, logika byłaby trudniejsza,
        // na razie zróbmy prosto: przyjmij tylko, jak jest pusto.
        if (transform.childCount > 0 && currentItem != null) 
        {
             return; // Slot zajęty, odrzuć
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            // 1. Mówimy przedmiotowi: "To jest Twój nowy dom" (zapobiega powrotowi)
            draggableItem.parentAfterDrag = transform;

            // 2. Aktualizujemy dane logiczne w tym slocie
            currentItem = draggableItem.itemData;
            
            // 3. Ustawiamy wizualia (na wszelki wypadek)
            iconDisplay.sprite = draggableItem.itemData.icon;
            iconDisplay.enabled = true;
        }
    }
}