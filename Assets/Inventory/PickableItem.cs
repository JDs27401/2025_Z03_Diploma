using C__Classes.Managers;
using Player.scripts;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickableItem : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1; 
    public WeaponInstanceState droppedWeaponState;

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
            if (itemData.itemType == ItemType.Collectible)
            {
                if (JournalManager.Instance != null)
                {
                    JournalManager.Instance.UnlockCollectible(itemData.id); // Adding only to the journal
                    isBeingPickedUp = true;
                    Destroy(gameObject);
                }
                
                return;
            }

            if (InventoryManager.Instance != null)
            {
                WeaponInstanceState stateToTransfer = droppedWeaponState != null ? droppedWeaponState.Clone() : null;
                bool wasPickedUp = InventoryManager.Instance.AddItem(itemData, amount, stateToTransfer);

                if (wasPickedUp)
                {
                    isBeingPickedUp = true;
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
}