using System;
using System.Collections.Generic;
using Player.scripts;
using UnityEngine;

namespace C__Classes.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public string savedAtUtc;
        public WorldSaveData world = new WorldSaveData();
        public PlayerSaveData player = new PlayerSaveData();
        public int selectedInventorySlotIndex;
        public List<InventorySlotSaveData> inventory = new List<InventorySlotSaveData>();
        public JournalSaveData journal = new JournalSaveData();
        public UniverseData universe = new UniverseData();
    }

    [Serializable]
    public class WorldSaveData
    {
        public string mapSeed;
        public string currentSceneName;
        public string targetSpawnId;
        public string returnSpawnId;
        public List<string> loadedAdditiveScenes = new List<string>();
        public List<string> lootedIds = new List<string>();
        public List<ContainerSaveData> containers = new List<ContainerSaveData>();
    }

    [Serializable]
    public class PlayerSaveData
    {
        public Vector3 position;
        public float currentHealth;
        public float currentStamina;
    }

    [Serializable]
    public class InventorySlotSaveData
    {
        public int slotIndex;
        public string itemId;
        public int count;
        public WeaponInstanceSaveData weaponState;
        public MeleeWeaponInstanceSaveData meleeWeaponState;
    }

    [Serializable]
    public class JournalSaveData
    {
        public List<string> unlockedCollectibleIds = new List<string>();
    }

    [Serializable]
    public class ContainerSaveData
    {
        public string containerId;
        public List<ContainerSlotSaveData> slots = new List<ContainerSlotSaveData>();
    }

    [Serializable]
    public class ContainerSlotSaveData
    {
        public int slotIndex;
        public string itemId;
        public int count;
        public WeaponInstanceSaveData weaponState;
        public MeleeWeaponInstanceSaveData meleeWeaponState;
    }

    [Serializable]
    public class WeaponInstanceSaveData
    {
        public int currentMagazineAmmo;
        public WeaponRuntimeStats runtimeStats;
        public List<string> installedModItemIds = new List<string>();
    }

    [Serializable]
    public class MeleeWeaponInstanceSaveData
    {
        public MeleeWeaponRuntimeStats runtimeStats;
        public List<string> installedModItemIds = new List<string>();
    }

    [Serializable]
    public class UniverseData
    {
        public int day;
        public int hour;
        public int minute;
    }
}
