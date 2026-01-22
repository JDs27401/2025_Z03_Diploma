using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            if (InventoryManager.instance != null)
            {
                bool wasPickedUp = InventoryManager.instance.AddItem(itemData, amount);

                if (wasPickedUp)
                {
                    Debug.Log($"Podniesiono: {itemData.itemName} x{amount}");
                    Destroy(gameObject);
                }
            }
        }
    }
}