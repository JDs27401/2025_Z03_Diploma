using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using C__Classes.Managers;
using C__Classes.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C__Classes.SaveSystem
{
    public class SaveGameManager : MonoBehaviour
    {
        public static ItemDatabase ActiveItemDatabase { get; private set; }

        [SerializeField] private string fileName = "savegame.json";
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private string itemDatabaseResourcePath = "ItemDatabase";
        [SerializeField] private PlayerController player;
        [SerializeField] private InventoryManager inventoryManager;

        public string SavePath => Path.Combine(Application.persistentDataPath, fileName);

        private void Awake()
        {
            ResolveItemDatabase();
            ActiveItemDatabase = itemDatabase;

            if (player == null)
            {
                player = FindFirstObjectByType<PlayerController>();
            }

            if (inventoryManager == null)
            {
                inventoryManager = InventoryManager.Instance;
            }
        }

        public void SaveGame()
        {
            if (!TryGetDependencies())
            {
                return;
            }

            SaveData saveData = new SaveData
            {
                savedAtUtc = DateTime.UtcNow.ToString("O"),
                world = CaptureWorldSaveData(),
                player = player.CaptureSaveData(),
                selectedInventorySlotIndex = inventoryManager.selectedSlotIndex,
                inventory = inventoryManager.CaptureInventorySaveData(),
                universe = CaptureUniverseState()
            };

            string json = JsonUtility.ToJson(saveData, true);
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            File.WriteAllText(SavePath, json);

            Debug.Log($"Game saved to {SavePath}");
        }

        public bool LoadGame()
        {
            if (!TryGetDependencies())
            {
                return false;
            }

            if (!File.Exists(SavePath))
            {
                Debug.LogWarning($"No save file found at {SavePath}");
                return false;
            }

            string json = File.ReadAllText(SavePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            if (saveData == null)
            {
                Debug.LogWarning($"Save file at {SavePath} could not be parsed.");
                return false;
            }

            StartCoroutine(LoadGameRoutine(saveData));
            return true;
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log($"Deleted save file at {SavePath}");
            }
        }

        private bool TryGetDependencies()
        {
            ResolveItemDatabase();

            if (player == null)
            {
                player = FindFirstObjectByType<PlayerController>();
            }

            if (inventoryManager == null)
            {
                inventoryManager = InventoryManager.Instance;
            }

            if (player == null || inventoryManager == null || itemDatabase == null)
            {
                Debug.LogWarning("[SaveGameManager] Missing player, inventory manager, or item database.");
                return false;
            }

            ActiveItemDatabase = itemDatabase;
            return true;
        }

        private void ResolveItemDatabase()
        {
            if (itemDatabase != null)
            {
                return;
            }

            itemDatabase = Resources.Load<ItemDatabase>(itemDatabaseResourcePath);
            if (itemDatabase == null)
            {
                Debug.LogWarning($"[SaveGameManager] ItemDatabase was not assigned and could not be loaded from Resources/{itemDatabaseResourcePath}.");
            }
        }

        private IEnumerator LoadGameRoutine(SaveData saveData)
        {
            RestoreWorldPersistence(saveData.world);

            if (saveData.world != null && saveData.world.loadedAdditiveScenes != null)
            {
                for (int i = 0; i < saveData.world.loadedAdditiveScenes.Count; i++)
                {
                    string sceneName = saveData.world.loadedAdditiveScenes[i];
                    if (string.IsNullOrWhiteSpace(sceneName) || SceneManager.GetSceneByName(sceneName).isLoaded)
                    {
                        continue;
                    }

                    AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    while (loadOperation != null && !loadOperation.isDone)
                    {
                        yield return null;
                    }
                }
            }

            player.RestoreSaveData(saveData.player);
            inventoryManager.selectedSlotIndex = saveData.selectedInventorySlotIndex;
            inventoryManager.RestoreInventoryFromSaveData(saveData.inventory, itemDatabase);
            RestoreUniverseState(saveData.universe);

            Debug.Log($"Game loaded from {SavePath}");
        }

        private WorldSaveData CaptureWorldSaveData()
        {
            WorldSaveData worldSaveData = new WorldSaveData
            {
                mapSeed = GetCurrentMapSeed(),
                currentSceneName = SceneManager.GetActiveScene().name,
                targetSpawnId = SceneTransport.TargetSpawnID,
                returnSpawnId = SceneTransport.ReturnSpawnID
            };

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != SceneManager.GetActiveScene())
                {
                    worldSaveData.loadedAdditiveScenes.Add(scene.name);
                }
            }

            if (LootManager.Instance != null)
            {
                LockerInteractable[] lockers = FindObjectsOfType<LockerInteractable>();
                for (int i = 0; i < lockers.Length; i++)
                {
                    if (lockers[i] != null)
                    {
                        lockers[i].SaveCurrentState();
                    }
                }

                worldSaveData.lootedIds = LootManager.Instance.CaptureLootedIds();
                worldSaveData.containers = LootManager.Instance.CaptureContainerStates();
            }

            return worldSaveData;
        }

        private void RestoreWorldPersistence(WorldSaveData worldSaveData)
        {
            if (worldSaveData == null)
            {
                return;
            }

            SceneTransport.TargetSpawnID = worldSaveData.targetSpawnId;
            SceneTransport.ReturnSpawnID = worldSaveData.returnSpawnId;

            if (MainMenuManager.Instance != null)
            {
                MainMenuManager.Instance.SetSeed(worldSaveData.mapSeed);
            }

            if (LootManager.Instance != null)
            {
                LootManager.Instance.RestoreLootedIds(worldSaveData.lootedIds);
                LootManager.Instance.RestoreContainerStates(worldSaveData.containers);
            }
        }

        private string GetCurrentMapSeed()
        {
            TilemapGenerationSystem tilemapGenerationSystem = FindFirstObjectByType<TilemapGenerationSystem>();
            if (tilemapGenerationSystem != null)
            {
                return tilemapGenerationSystem.GetSeed();
            }

            return MainMenuManager.Instance != null ? MainMenuManager.Instance.Seed : string.Empty;
        }

        private UniverseData CaptureUniverseState()
        {
            UniverseData universeData = new UniverseData
            {
                day = Universe.GetDay(),
                hour = 8,
                minute = 0
            };
            return universeData;
        }

        private void RestoreUniverseState(UniverseData universeData)
        {
            if (universeData == null)
            {
                return;
            }

            if (Universe.Instance == null)
            {
                return;
            }
            Universe.SetDay(universeData.day);
            Universe.SetHour(universeData.hour);
            Universe.SetMinute(universeData.minute);
        }
    }
}
