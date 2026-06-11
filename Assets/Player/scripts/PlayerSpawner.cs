using C__Classes.SceneManagement;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        string targetScene = SceneTransport.TargetSpawnID;
        
        if (string.IsNullOrEmpty(targetScene)) return;

        GameObject targetObject = null;
        
        targetObject = GameObject.Find(targetScene);
        
        if (targetObject == null)
        {
            SceneManagment[] allDoors = FindObjectsOfType<SceneManagment>();
            foreach (var door in allDoors)
            {
                if (door.myUniqueID == targetScene)
                {
                    targetObject = door.gameObject;
                    break;
                }
            }
        }
        
        if (targetObject != null)
        {
            transform.position = targetObject.transform.position;
        }
        else
        {
            Debug.LogWarning($"Target not found: {targetScene}");
        }
        
        SceneTransport.TargetSpawnID = null;
    }
}
