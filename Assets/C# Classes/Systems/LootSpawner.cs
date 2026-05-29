using System.Collections.Generic;
using UnityEngine;

namespace C__Classes.Systems
{
    public class LootSpawner : MonoBehaviour
    {
        public GameObject[] possibleLootPrefabs;

        public Transform[] spawnPoints;

        public float spawnChance = 0.5f;

        [Header("Szansa na trafienie w procentach")]
        public float commonChance = 60f;
        public float rareChance = 20f;
        public float unusualChance = 15f;
        public float uniqueChance = 5f;

        private void Start()
        {
            int seed = SceneTransport.ReturnSpawnID.GetHashCode();
            
            Random.InitState(seed);

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (Random.value < spawnChance)
                {
                    SpawnLoot(spawnPoint);
                }
            }
        }

        private void SpawnLoot(Transform spawnPoint)
        {
            string selectRarity = GetRandomRarity();
            
            List<GameObject> matchingLoot = new List<GameObject>();

            foreach (GameObject prefab in possibleLootPrefabs)
            {
                PickableItem pickable = prefab.GetComponent<PickableItem>();

                if (pickable.itemData.rarity.ToString() == selectRarity)
                {
                    matchingLoot.Add(prefab);
                }
            }

            if (matchingLoot.Count > 0)
            {
                int randomIndex = Random.Range(0, matchingLoot.Count);
                Instantiate(matchingLoot[randomIndex], spawnPoint.position, Quaternion.identity, spawnPoint);
            }
        }

        private string GetRandomRarity()
        {
            float totalWeight = commonChance + rareChance + unusualChance + uniqueChance;
            float randomValue = Random.Range(0, totalWeight);

            if (randomValue < commonChance) return "Common";
            randomValue -= commonChance;

            if (randomValue < rareChance) return "Rare";
            randomValue -= rareChance;

            if (randomValue < unusualChance) return "Unusual";
            
            return "Unique";
        }
    }
}