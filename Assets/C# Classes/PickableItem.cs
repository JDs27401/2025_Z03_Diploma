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
        Debug.Log($"[Tracer] {gameObject.name} wszedł w Trigger z: {other.gameObject.name} (Tag: {other.tag})");
        
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
        Debug.Log($"[Tracer] {gameObject.name} uderzył w kolider twardy: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
    }

    private void OnDisable()
    {
        if (!isBeingPickedUp && gameObject.scene.isLoaded)
        {
            Debug.LogError($"[UWAGA] Obiekt {gameObject.name} został wyłączony lub zniszczony! Sprawdź logi wyżej, aby zobaczyć, z czym przed chwilą kolidował.");
        }
    }
}