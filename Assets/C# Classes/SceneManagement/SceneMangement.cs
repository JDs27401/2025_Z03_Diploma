using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("Ustawienia")] 
    public string sceneToLoad;
    
    [Header("ID spawnu w nowej scenie")]
    public string targetSpawnID;
    
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
        
        SceneTransport.TargetSpawnID = targetSpawnID;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            isPlayerInRange = false;
        }
    }
}
