using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CraftingZone : MonoBehaviour
{
    public GameObject craftingUI;

    private bool isPlayerInRange = false;
    private PlayerInteractionUI playerUI;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (craftingUI != null)
            {
                craftingUI.SetActive(!craftingUI.activeSelf);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true;
            
            playerUI = other.GetComponent<PlayerInteractionUI>();
            if (playerUI != null)
            {
                playerUI.AddInteractable(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false;
            
            if (craftingUI != null)
            {
                craftingUI.SetActive(false);
            }

            if (playerUI != null)
            {
                playerUI.RemoveInteractable(gameObject);
                playerUI = null; 
            }
        }
    }
}