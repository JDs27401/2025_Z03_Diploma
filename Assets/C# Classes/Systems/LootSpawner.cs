using UnityEngine;

namespace C__Classes.Systems
{
    public class LootSpawner : MonoBehaviour
    {
        public GameObject[] possibleLootPrefabs;

        public Transform[] spawnPoints;

        public float spawnChance = 0.5f;

        private void Start()
        {
            int seed = SceneTransport.ReturnSpawnID.GetHashCode();
            
            Random.InitState(seed);

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (Random.value < spawnChance)
                {
                    int randomIndex = Random.Range(0, possibleLootPrefabs.Length);
                    Instantiate(possibleLootPrefabs[randomIndex], spawnPoint.position, Quaternion.identity, spawnPoint);
                }
            }
        }
    }
}