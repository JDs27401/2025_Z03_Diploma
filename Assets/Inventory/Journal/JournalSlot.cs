using UnityEngine;
using UnityEngine.UI;

public class JournalSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    
    private ItemData assignedItem;
    private bool isUnlocked = false;

    // Inicjalizacja slota przypisanym przedmiotem
    public void Setup(ItemData item)
    {
        assignedItem = item;
        
        if (assignedItem != null && assignedItem.icon != null)
        {
            iconImage.sprite = assignedItem.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        UpdateVisuals();
    }

    // Aktualizacja stanu odblokowania
    public void SetUnlocked(bool state)
    {
        isUnlocked = state;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (iconImage == null || assignedItem == null) return;

        if (isUnlocked)
        {
            iconImage.color = Color.white; // Pełen kolor
        }
        else
        {
            // Wyszarzenie - możesz dostosować te wartości (np. ciemnoszary, lekko przezroczysty)
            iconImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); 
        }
    }
}