using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // Lista slotów w UI (przypiszemy je w Inspectorze)
    public InventorySlot[] slots;

    // Lista Twoich przedmiotów .asset, które chcesz dać graczowi na start
    // UWAGA: Tutaj też zmień 'ScriptableObject' na nazwę Twojej klasy (np. ItemData)
    public List<ScriptableObject> startingItems;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Pętla przez wszystkie sloty
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < startingItems.Count)
            {
                // Mamy przedmiot dla tego slota!
                // Tutaj musimy pobrać ikonę z Twojego .asset.
                // Zakładam, że w Twoim skrypcie .asset pole z ikoną nazywa się 'icon'.
                // Jeśli nie masz dostępu bezpośrednio, musisz rzutować typ:
                
                // Przykład (odkomentuj i dopasuj do swojej klasy):
                // YourItemClass item = (YourItemClass)startingItems[i];
                // slots[i].AddItem(item, item.icon);
                
                // WERSJA TYMCZASOWA (jeśli nie podasz mi nazwy swojej klasy):
                Debug.Log("Dodano przedmiot do slotu: " + i);
                // slots[i].AddItem(startingItems[i], null); // Tu potrzebujesz Sprite'a z Twojego assetu
            }
            else
            {
                // Brak przedmiotu dla tego slota -> wyczyść go
                slots[i].ClearSlot();
            }
        }
    }

    public bool AddItemToFirstFreeSlot(ScriptableObject item, Sprite icon)
{
    // Przejdź przez wszystkie sloty
    foreach (InventorySlot slot in slots)
    {
        if (slot.IsEmpty())
        {
            slot.AddItem(item, icon);
            return true; // Sukces! Przedmiot dodany
        }
    }
    
    Debug.Log("Ekwipunek jest pełny!");
    return false; // Brak miejsca
}
}