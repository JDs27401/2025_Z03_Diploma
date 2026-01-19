using C__Classes.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C__Classes.SceneManagement
{
    public class SceneManagement : MonoBehaviour
    {
        [Header("Ustawienia")] public string sceneToLoad;
        [SerializeField] private string MainScene;

        [Header("ID spawnu w nowej scenie")] public string targetSpawnID;

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
            if (SceneManager.GetActiveScene().name == MainScene)
            {
                SceneTransport.TargetSpawnID = targetSpawnID;
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
                SceneManager.sceneLoaded += SceneManagerOnsceneLoaded; 
                
            } else if (SceneManager.GetActiveScene().name == sceneToLoad)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(MainScene));
                SceneTransport.TargetSpawnID = targetSpawnID;
                SceneManager.UnloadSceneAsync(sceneToLoad);
                
            }
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != sceneToLoad) return;
            
            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
            SceneManager.SetActiveScene(scene);
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
}
