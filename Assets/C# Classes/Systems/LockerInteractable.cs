using UnityEngine;
using C__Classes.Systems; 

public class LockerInteractable : MonoBehaviour
{
    [Header("Ustawienia Lootu")]
    public GameObject[] possibleLootPrefabs;
    public float spawnChance = 0.5f;

    [Header("Stan Szafki")]
    public GameObject[] slotItems = new GameObject[2];
    
    private string lockerUniqueID;
    private bool isPlayerInRange = false;

    private void Start()
    {
        string buildingID = SceneTransport.ReturnSpawnID;
        string pos = Mathf.RoundToInt(transform.position.x) + "_" + Mathf.RoundToInt(transform.position.y);
        lockerUniqueID = $"{buildingID}_Locker_{pos}";
        
        int seed = lockerUniqueID.GetHashCode();
        Random.InitState(seed);

        for (int i = 0; i < 2; i++)
        {
            string slotID = GetSlotID(i);
            
            if (LootManager.Instance != null && LootManager.Instance.IsAlreadyLooted(slotID))
            {
                slotItems[i] = null;
                continue;
            }
            
            if (Random.value < spawnChance && possibleLootPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, possibleLootPrefabs.Length);
                slotItems[i] = possibleLootPrefabs[randomIndex];
            }
            else
            {
                slotItems[i] = null;
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            LockerUIManager.Instance.OpenLockerUI(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false;
            if (LockerUIManager.Instance != null) LockerUIManager.Instance.CloseUI();
        }
    }
    
    public string GetSlotID(int slotIndex)
    {
        return $"{lockerUniqueID}_Slot{slotIndex}";
    }
}