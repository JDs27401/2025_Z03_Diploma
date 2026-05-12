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
    
    private bool isUIOpen = false; 
    
    private PlayerInteractionUI playerUI;

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
            if (isUIOpen)
            {
                if (LockerUIManager.Instance != null) LockerUIManager.Instance.CloseUI();
                isUIOpen = false;
                
                if (playerUI != null) playerUI.AddInteractable(gameObject);
            }
            else
            {
                LockerUIManager.Instance.OpenLockerUI(this);
                isUIOpen = true;
                
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
}