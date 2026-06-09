using System;
using System.Collections.Generic;
using C__Classes.SaveSystem;
using C__Classes.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

public class LockerInteractable : MonoBehaviour
{
    [Header("Ustawienia Lootu")]
    public GameObject[] possibleLootPrefabs;
    public float spawnChance = 0.5f;

    [Header("Stan Szafki")]
    public GameObject[] slotItems = new GameObject[2];
    private readonly ContainerSlotSaveData[] slotStates = new ContainerSlotSaveData[2];

    [Header("Szansa na trafienie w procentach")]
    public float commonChance = 60f;
    public float rareChance = 20f;
    public float unusualChance = 15f;
    public float uniqueChance = 5f;

    private string lockerUniqueID;
    private bool isPlayerInRange = false;
    private bool isUIOpen = false;
    private PlayerInteractionUI playerUI;

    public event Action OnChestOpen;
    public event Action OnChestClosed;

    private void Start()
    {
        lockerUniqueID = BuildContainerId();

        if (LootManager.Instance != null && LootManager.Instance.TryGetContainerState(lockerUniqueID, out ContainerSaveData savedContainer))
        {
            RestoreFromSaveData(savedContainer);
            return;
        }

        int seed = MapGenerationSystem.GenerateSeed(lockerUniqueID);
        Random.InitState(seed);

        for (int i = 0; i < slotStates.Length; i++)
        {
            string slotID = GetSlotID(i);

            if (LootManager.Instance != null && LootManager.Instance.IsAlreadyLooted(slotID))
            {
                SetSlotState(i, CreateEmptySlotState(i));
                continue;
            }

            GameObject selectedLoot = null;
            if (Random.value < spawnChance && possibleLootPrefabs.Length > 0)
            {
                selectedLoot = SpawnLootBasedOnRarity();
            }

            slotItems[i] = selectedLoot;
            SetSlotState(i, CreateSlotStateFromPrefab(i, selectedLoot));
        }

        SaveCurrentState();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isUIOpen)
            {
                if (LockerUIManager.Instance != null) LockerUIManager.Instance.CloseUI();
                isUIOpen = false;

                OnChestClosed?.Invoke();

                if (playerUI != null) playerUI.AddInteractable(gameObject);
            }
            else
            {
                LockerUIManager.Instance.OpenLockerUI(this);
                isUIOpen = true;

                OnChestOpen?.Invoke();

                if (playerUI != null) playerUI.RemoveInteractable(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true;

            playerUI = other.GetComponent<PlayerInteractionUI>();
            if (playerUI != null)
            {
                playerUI.AddInteractable(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false;

            if (isUIOpen)
            {
                if (LockerUIManager.Instance != null) LockerUIManager.Instance.CloseUI();
                isUIOpen = false;
                OnChestClosed?.Invoke();
            }

            if (playerUI != null)
            {
                playerUI.RemoveInteractable(gameObject);
                playerUI = null;
            }
        }
    }

    public string GetSlotID(int slotIndex)
    {
        return $"{lockerUniqueID}_Slot{slotIndex}";
    }

    public ContainerSlotSaveData GetSlotState(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotStates.Length)
        {
            return null;
        }

        return slotStates[slotIndex];
    }

    public void SetSlotState(int slotIndex, ContainerSlotSaveData slotSaveData)
    {
        if (slotIndex < 0 || slotIndex >= slotStates.Length)
        {
            return;
        }

        slotStates[slotIndex] = slotSaveData ?? CreateEmptySlotState(slotIndex);
        slotStates[slotIndex].slotIndex = slotIndex;
        slotItems[slotIndex] = null;
    }

    public ContainerSaveData CaptureSaveData()
    {
        ContainerSaveData saveData = new ContainerSaveData
        {
            containerId = lockerUniqueID
        };

        for (int i = 0; i < slotStates.Length; i++)
        {
            saveData.slots.Add(slotStates[i] ?? CreateEmptySlotState(i));
        }

        return saveData;
    }

    public void SaveCurrentState()
    {
        if (LootManager.Instance != null)
        {
            LootManager.Instance.SaveContainerState(CaptureSaveData());
        }
    }

    private void RestoreFromSaveData(ContainerSaveData saveData)
    {
        for (int i = 0; i < slotStates.Length; i++)
        {
            SetSlotState(i, CreateEmptySlotState(i));
        }

        if (saveData == null || saveData.slots == null)
        {
            return;
        }

        for (int i = 0; i < saveData.slots.Count; i++)
        {
            ContainerSlotSaveData slotSaveData = saveData.slots[i];
            if (slotSaveData == null || slotSaveData.slotIndex < 0 || slotSaveData.slotIndex >= slotStates.Length)
            {
                continue;
            }

            SetSlotState(slotSaveData.slotIndex, slotSaveData);
        }
    }

    private string BuildContainerId()
    {
        string buildingID = string.IsNullOrWhiteSpace(SceneTransport.ReturnSpawnID)
            ? gameObject.scene.name
            : SceneTransport.ReturnSpawnID;
        string pos = Mathf.RoundToInt(transform.position.x) + "_" + Mathf.RoundToInt(transform.position.y);
        return $"{buildingID}_Locker_{pos}";
    }

    private ContainerSlotSaveData CreateSlotStateFromPrefab(int slotIndex, GameObject itemPrefab)
    {
        ContainerSlotSaveData slotState = CreateEmptySlotState(slotIndex);
        if (itemPrefab == null)
        {
            return slotState;
        }

        PickableItem pickable = itemPrefab.GetComponent<PickableItem>();
        if (pickable == null || pickable.itemData == null)
        {
            return slotState;
        }

        slotState.itemId = pickable.itemData.id;
        slotState.count = pickable.amount > 0 ? pickable.amount : 1;
        slotState.weaponState = SaveStateMapper.CaptureWeaponState(pickable.GetDroppedWeaponStateClone());
        slotState.meleeWeaponState = SaveStateMapper.CaptureMeleeWeaponState(pickable.GetDroppedMeleeStateClone());
        return slotState;
    }

    private ContainerSlotSaveData CreateEmptySlotState(int slotIndex)
    {
        return new ContainerSlotSaveData
        {
            slotIndex = slotIndex,
            itemId = null,
            count = 0
        };
    }

    private GameObject SpawnLootBasedOnRarity()
    {
        string selectedRarity = GetRandomRarity();
        List<GameObject> matchingLoot = new List<GameObject>();

        foreach (GameObject prefab in possibleLootPrefabs)
        {
            PickableItem pickable = prefab.GetComponent<PickableItem>();

            if (pickable != null && pickable.itemData != null && pickable.itemData.rarity.ToString() == selectedRarity)
            {
                matchingLoot.Add(prefab);
            }
        }

        if (matchingLoot.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingLoot.Count);
            return matchingLoot[randomIndex];
        }

        return null;
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
