using System;
using System.Collections;
using System.Collections.Generic;
using C__Classes.Objects;
using C__Classes.Singletons;
using C__Classes.Systems;
using UnityEngine;

namespace C__Classes.Managers
{
    public class WaveManager : SingletonNonPersistant<WaveManager>
    {
        public event Action OnWaveCompleted;
        
        private int _aliveEnemies;
        private List<GameObject> _enemies = new List<GameObject>();
        public bool AlreadyStarted { get; private set; }  = false;

        [Header("Max enemies spawned at the same time")] 
        [SerializeField] private int mesatst = 500;
        private int _remainingEnemiesToSpawn = 0;
        
        [Header("Wave Settings")]
        [SerializeField] private GameObject[] spawnpoints;
        [SerializeField] private int waveSize = 0;
        [SerializeField] private float waveSizeMultiplier = 5f;
        [SerializeField] private float spawnDelta = 1.0f;

        [Header("Enemy Prefab List")]
        [SerializeField] private GameObject[] enemies;

        private IEnumerator StartWave()
        {
            #if UNITY_EDITOR
            print("Wave started");
            #endif
            AlreadyStarted = true;
            waveSize += CalculateAdditionalEnemies();
            if (waveSize > mesatst)
            {
                waveSize = mesatst;
                _remainingEnemiesToSpawn = waveSize - mesatst;
            }
            
            for (int i = 0; i < waveSize; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelta);
            }
            // waveSize = (int) (waveSize * waveSizeMultiplier);
        }

        public void StartWaveCoroutine()
        {
            if (enemies.Length <= 0)
            {
                WaveCompleted();
                return;
            }
            StartCoroutine(StartWave());
        }
        
        private void HandleEnemyDeath(GameObject go)
        {
            go.GetComponent<WaveComponent>().OnDeath -= HandleEnemyDeath;
            _enemies.Remove(go);
            _aliveEnemies--;

            if (_remainingEnemiesToSpawn != 0)
            {
                SpawnEnemy();
                _remainingEnemiesToSpawn--;
                return;
            }

            if (_aliveEnemies <= 0)
            {
                WaveCompleted();
            }
        }
        
        private void WaveCompleted()
        {
            #if UNITY_EDITOR
            print("Wave completed");
            #endif
            OnWaveCompleted?.Invoke();
            AlreadyStarted = false;
        }

        private void SpawnEnemy()
        {
            var random = new System.Random();
            GameObject enemy = Instantiate(enemies[random.Next(enemies.Length)], spawnpoints[random.Next(spawnpoints.Length)].transform.position, Quaternion.identity);
            _aliveEnemies++;
            enemy.GetComponent<WaveComponent>().OnDeath += HandleEnemyDeath;
            _enemies.Add(enemy);
        }

        private int CalculateAdditionalEnemies()
        {
            return (int) (waveSizeMultiplier * Math.Pow(Universe.GetDay(), 1.75));
        }
    }
}
