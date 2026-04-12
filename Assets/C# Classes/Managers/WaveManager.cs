using System;
using System.Collections;
using System.Collections.Generic;
using C__Classes.Objects;
using C__Classes.Singletons;
using UnityEngine;

namespace C__Classes.Managers
{
    public class WaveManager : SingletonNonPersistant<WaveManager>
    {
        public event Action OnWaveCompleted;
        
        private float _aliveEnemies;
        private List<GameObject> _enemies;
        public bool AlreadyStarted { get; private set; }  = false;

        [Header("Wave Settings")]
        [SerializeField] private GameObject[] spawnpoints;
        [SerializeField] private int waveSize = 0;
        [SerializeField] private float waveSizeMultiplier = 1.5f;
        [SerializeField] private float spawnDelta = 1.0f;

        [Header("Enemy Prefab List")]
        [SerializeField] private GameObject[] enemies;

        public IEnumerator StartWave()
        {
            #if UNITY_EDITOR
            print("Wave started");
            #endif
            AlreadyStarted = true;
            for (int i = 0; i < waveSize; i++)
            {
                var random = new System.Random();
                GameObject enemy = Instantiate(enemies[random.Next(enemies.Length)], spawnpoints[random.Next(spawnpoints.Length)].transform.position, Quaternion.identity);
                yield return new WaitForSeconds(spawnDelta);
                _aliveEnemies++;
                enemy.GetComponent<WaveComponent>().OnDeath += HandleEnemyDeath;
                _enemies.Add(enemy);
            }
            waveSize = (int) (waveSize * waveSizeMultiplier);
        }
        
        private void HandleEnemyDeath()
        {
            _aliveEnemies--;

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

        private void OnDestroy()
        {
            foreach (var e in _enemies)
            {
                e.GetComponent<WaveComponent>().OnDeath -= HandleEnemyDeath;
            }
        }
    }
}
