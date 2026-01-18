using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        if (!string.IsNullOrEmpty(SceneTransport.TargetSpawnID))
        {
            GameObject target = GameObject.Find(SceneTransport.TargetSpawnID);

            if (target != null)
            {
                transform.position = target.transform.position;
                Debug.Log($"Sukces, znalazłem {target.name}");
            }
            //If there's no spawn point of that name, Player will simply be taken into the scene 
        }
        SceneTransport.TargetSpawnID = null;
    }
}
