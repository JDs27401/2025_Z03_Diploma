using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public ItemData itemData; // Twoje dane z pliku .asset

    // Ta funkcja wywoła się AUTOMATYCZNIE, gdy ktoś wejdzie w ten obiekt
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy to GRACZ wszedł w przedmiot (a nie np. potwór czy ściana)
        if (other.CompareTag("player"))
        {
            Debug.Log("Gracz dotknął przedmiotu!");

            InventoryManager manager = FindObjectOfType<InventoryManager>();
            
            if (manager != null)
            {
                // Próbujemy dodać do ekwipunku
                bool wasPickedUp = manager.AddItemToFirstFreeSlot(itemData, itemData.icon);

                // Jeśli się udało -> zniszcz przedmiot na scenie
                if (wasPickedUp)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}