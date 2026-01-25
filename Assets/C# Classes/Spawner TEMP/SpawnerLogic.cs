using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace C__Classes.Spawner_TEMP
{
    public class SpawnerLogic :  MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int spawnAmount;
        [SerializeField] private int interval;
        [SerializeField] private bool isActive;
        private bool canSpawn;

        private void Start()
        {
            isActive = true;
            canSpawn = true;
        }

        private void FixedUpdate()
        {
            if (!isActive)
            {
                return;
            }

            if (!canSpawn)
            {
                return;
            }

            Random random = new Random();
            for (int i = 0; i < spawnAmount; i++)
            {
                Vector2 position = new Vector2(random.Next(-1, 1), random.Next(-1, 1));
                Instantiate(prefab, transform.position + (Vector3)position, Quaternion.identity);
            }
            
            StartCoroutine(CanSpawn());
        }

        private IEnumerator CanSpawn()
        {
            canSpawn = false;
            yield return new WaitForSeconds(interval);
            canSpawn = true;
        }
    }
}