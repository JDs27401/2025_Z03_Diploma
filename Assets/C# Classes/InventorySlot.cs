using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconDisplay; // Przeciągniesz tu obrazek "Icon" (dziecko slota)
    
    // Zmienna przechowująca Twój plik .asset (dane)
    // UWAGA: Zmień 'ItemData' na nazwę Twojej klasy ScriptableObject (np. Item, ItemObject itp.)
    private ScriptableObject currentItem; 

    // Funkcja do dodania przedmiotu do tego slota
    public void AddItem(ScriptableObject newItem, Sprite itemIcon)
    {
        currentItem = newItem;
        iconDisplay.sprite = itemIcon;
        iconDisplay.enabled = true; // Włączamy obrazek
    }

    // Funkcja do wyczyszczenia slota
    public void ClearSlot()
    {
        currentItem = null;
        iconDisplay.sprite = null;
        iconDisplay.enabled = false; // Wyłączamy obrazek, żeby nie było białego kwadratu
    }
    
    public bool IsEmpty()
{
    return currentItem == null;
}
}