using UnityEngine;
using UnityEngine.UI;

public class JournalSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public Outline itemOutline; // <--- NOWE POLE
    
    private ItemData assignedItem;
    private bool isUnlocked = false;

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
            
            // Wyłączamy outline po podniesieniu przedmiotu
            if (itemOutline != null) 
                itemOutline.enabled = false; 
        }
        else
        {
            iconImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Wyszarzenie
            
            // Włączamy biały outline, gdy przedmiot jest nieodkryty
            if (itemOutline != null) 
            {
                itemOutline.enabled = true;
                itemOutline.effectColor = Color.white; // Ustawienie koloru na biały
            }
        }
    }
}