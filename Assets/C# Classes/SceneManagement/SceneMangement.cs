using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("Ustawienia")] 
    public string sceneToLoad;
    
    [Header("ID spawnu w nowej scenie (wypelniamy go tylko przy wchodzeniu do budynku, tak to zostawiamy puste)")]
    public string targetSpawnID;

    //We automatically set a unique ID for each trigger point
    // [HideInInspector]
    public string myUniqueID;
    
    private bool isPlayerInRange = false;

    private void Awake()
    {
        //We generate unique ID
        myUniqueID = Guid.NewGuid().ToString();
    }
    
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EnterDoor());
        }
    }

    private IEnumerator EnterDoor()
    {
        //if statement is true when we are entering a building, and else while we are leaving the building
        //that's why it's important to NOT set targetSpawnID on spawnPoints in the interior of a building
        
        //we can also leave Scene To Load field empty for spawn points in interior, as right now
        //when leaving Unity will close the interior scene
        if (!string.IsNullOrEmpty(targetSpawnID))
        {
            SceneTransport.TargetSpawnID = targetSpawnID;
            SceneTransport.ReturnSpawnID = myUniqueID;

            if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
                
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
            }
        }
        else
        {
            SceneTransport.TargetSpawnID = SceneTransport.ReturnSpawnID;

            MovePlayerToReturnPoint();
            
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }
    }

    private void MovePlayerToReturnPoint()
    {
        SceneManagement[] allDoors = FindObjectsOfType<SceneManagement>();
        foreach (var door in allDoors)
        {
            if (door.myUniqueID == SceneTransport.ReturnSpawnID)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = door.transform.position;
                }
                break;
            }
        }
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
