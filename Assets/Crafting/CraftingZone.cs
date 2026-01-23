using UnityEngine;

public class CraftingZone : MonoBehaviour
{
    public GameObject craftingUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Wejście do strefy craftingu");
            if(craftingUI != null)
            {
                craftingUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Wyjście ze strefy craftingu");
            if (craftingUI != null)
            {
                craftingUI.SetActive(false);
            }
        }
    }
}