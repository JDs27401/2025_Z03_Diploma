using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickableItem : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1; 

    private bool isPlayerInRange = false;
    private bool isBeingPickedUp = false;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // --- ZABEZPIECZENIE: Sprawdzamy czy gracz aktualnie przeciąga inny przedmiot ---
            if (DraggableItem.isDraggingItem)
            {
                Debug.Log("[PickableItem] Nie można podnieść przedmiotu, przeciągasz inny przedmiot w ekwipunku.");
                return;
            }

            // --- 1. SPRAWDZENIE CZY TO ZNAJDŹKA (OMIJAMY EKWIPUNEK) ---
            if (itemData.itemType == ItemType.Collectible)
            {
                if (JournalManager.Instance != null)
                {
                    JournalManager.Instance.UnlockCollectible(itemData.id); // Dodajemy tylko do dziennika
                    isBeingPickedUp = true;
                    Debug.Log($"[Journal] Odkryto nową znajdźkę: {itemData.itemName}");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("Brak JournalManager na scenie! Znajdźka nie została podniesiona.");
                }
                
                return; // Przerywamy funkcję, by kod nie przeszedł do normalnego ekwipunku
            }

            // --- 2. JEŚLI TO NORMALNY PRZEDMIOT, TRAFIA DO EKWIPUNKU ---
            if (InventoryManager.instance != null)
            {
                bool wasPickedUp = InventoryManager.instance.AddItem(itemData, amount);

                if (wasPickedUp)
                {
                    isBeingPickedUp = true;
                    Debug.Log($"[Inventory] Podniesiono: {itemData.itemName} x{amount}");
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug można usunąć, żeby nie śmiecił konsoli
        // Debug.Log($"[Tracer] {gameObject.name} uderzył w kolider twardy: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
    }

    private void OnDisable()
    {
        if (!isBeingPickedUp && gameObject.scene.isLoaded)
        {
            Debug.LogError($"[UWAGA] Obiekt {gameObject.name} został wyłączony lub zniszczony! Sprawdź logi wyżej, aby zobaczyć, z czym przed chwilą kolidował.");
        }
    }
}