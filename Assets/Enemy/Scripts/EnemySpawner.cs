using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Enemy.Scripts
{
    [System.Serializable]
    public class EnemyConfig
    {
        public string name = "Enemy";
        public GameObject prefab;
        [Tooltip("Credit cost")]
        public int intensityCost = 1;
        [Tooltip("Spawnować?")]
        public bool isEnabled = true;
    }

    public enum SpawnMode
    {
        Regular,
        Waves
    }

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Ustawienia Ogólne")]
        [Tooltip("Maksymalna liczba creditów do wykorzystania w ciągu minuty")]
        public int intensityPerMinute = 60;
        
        [Tooltip("Tryb spawnowania")]
        public SpawnMode spawnMode = SpawnMode.Regular;

        [Header("Ustawienia Fal")]
        [Tooltip("Na ile fal podzielić zaplanowanych przeciwników w ciągu minuty (tylko dla trybu waves)")]
        public int waveCount = 4;

        [Header("Konfiguracja Przeciwników")]
        public List<EnemyConfig> enemyTypes = new List<EnemyConfig>();

        [Header("Pozycja Spawnowania")]
        [Tooltip("Minimalna odległość od gracza (poza ekranem).")]
        public float minSpawnRadius = 10f;
        [Tooltip("Maksymalna odległość od gracza.")]
        public float maxSpawnRadius = 15f;

        private Transform playerTransform;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("EnemySpawner: Nie znaleziono gracza z tagiem 'Player'. Spawnowanie będzie wokół punktu (0,0,0).");
            }

            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                // kolejka spawnu
                List<EnemyConfig> minuteSpawnList = GenerateSpawnListForMinute();
                ShuffleList(minuteSpawnList);

                // spawn
                if (spawnMode == SpawnMode.Regular)
                {
                    yield return StartCoroutine(RunRegularMode(minuteSpawnList));
                }
                else
                {
                    yield return StartCoroutine(RunWaveMode(minuteSpawnList));
                }

                // jeśli lista była pusta i coroutine skończyła się od razu
                if (minuteSpawnList.Count == 0)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        private List<EnemyConfig> GenerateSpawnListForMinute()
        {
            List<EnemyConfig> spawnList = new List<EnemyConfig>();
            int currentCost = 0;

            var available = enemyTypes.Where(e => e.isEnabled && e.prefab != null).ToList();
            if (available.Count == 0) return spawnList;

            int attempts = 0;
            while (currentCost < intensityPerMinute && attempts < 1000) //spawni max 1000 przeciwników per minute
            {
                var candidates = available.Where(e => (currentCost + Mathf.Max(1, e.intensityCost)) <= intensityPerMinute).ToList();
                
                if (candidates.Count == 0)
                {
                    break;
                }

                EnemyConfig selected = candidates[Random.Range(0, candidates.Count)];
                spawnList.Add(selected);
                
                // Zabezpieczenie przed kosztem 0 lub ujemnym
                currentCost += Mathf.Max(1, selected.intensityCost);
                
                attempts++;
            }

            return spawnList;
        }

        private IEnumerator RunRegularMode(List<EnemyConfig> enemies)
        {
            if (enemies.Count == 0) yield break;

            float duration = 60f;
            float interval = duration / enemies.Count;

            foreach (var enemy in enemies)
            {
                SpawnEnemy(enemy);
                yield return new WaitForSeconds(interval);
            }
        }

        private IEnumerator RunWaveMode(List<EnemyConfig> enemies)
        {
            if (enemies.Count == 0) yield break;

            float duration = 60f;
            int waves = Mathf.Max(1, waveCount);
            float interval = duration / waves;
            
            List<List<EnemyConfig>> wavePacks = new List<List<EnemyConfig>>();
            for (int i = 0; i < waves; i++) wavePacks.Add(new List<EnemyConfig>());

            for (int i = 0; i < enemies.Count; i++)
            {
                wavePacks[i % waves].Add(enemies[i]);
            }

            // Wykonujemy fale
            for (int i = 0; i < waves; i++)
            {
                foreach (var enemy in wavePacks[i])
                {
                    SpawnEnemy(enemy);
                }
                yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnEnemy(EnemyConfig config)
        {
            Vector3 pos = GetSpawnPosition();
            Instantiate(config.prefab, pos, Quaternion.identity);
        }

        private Vector3 GetSpawnPosition()
        {
            Vector3 center = playerTransform ? playerTransform.position : Vector3.zero;
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnRadius, maxSpawnRadius);
            
            return center + new Vector3(randomDir.x, randomDir.y, 0) * dist;
        }

        private void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1) 
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = Application.isPlaying && playerTransform ? playerTransform.position : transform.position;
            Gizmos.DrawWireSphere(center, minSpawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, maxSpawnRadius);
        }
    }
}



