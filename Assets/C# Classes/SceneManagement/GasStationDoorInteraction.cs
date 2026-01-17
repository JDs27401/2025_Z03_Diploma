using UnityEngine;
using UnityEngine.SceneManagement;

public class GasStationDoorInteraction : MonoBehaviour
{
    [Header("Ustawienia")] 
    public string sceneToLoad;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            EnterDoor();
        }
    }

    private void EnterDoor()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            isPlayerInRange = true;
        }
    }
    
}
