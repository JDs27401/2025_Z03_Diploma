using UnityEngine;

public class CraftingZone : MonoBehaviour
{
    // Tutaj przeciągniesz swój panel craftingu (cały obiekt UI)
    public GameObject craftingUI;

    // Kiedy gracz WEJDZIE w strefę
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Wejście do strefy craftingu");
            if(craftingUI != null)
            {
                craftingUI.SetActive(true); // Pokaż panel
            }
        }
    }

    // Kiedy gracz WYJDZIE ze strefy
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Wyjście ze strefy craftingu");
            if (craftingUI != null)
            {
                craftingUI.SetActive(false); // Ukryj panel
            }
        }
    }
}