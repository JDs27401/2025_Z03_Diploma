using C__Classes.Managers;
using UnityEngine;
using UnityEngine.UI;
using C__Classes.Systems;
using Player.scripts;

namespace C__Classes.Systems
{
public class LockerUIManager : MonoBehaviour
{
    public static LockerUIManager Instance { get; private set; }

    [Header("Elementy UI")]
    public GameObject lockerUIPanel; 
    public Button[] slotButtons;     
    public Image[] slotIcons;        

    private LockerInteractable currentLocker;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lockerUIPanel.SetActive(false); 
    }

    public void OpenLockerUI(LockerInteractable locker)
    {
        currentLocker = locker;
        lockerUIPanel.SetActive(true);

        RefreshUI();
    }

    public void CloseUI()
    {
        lockerUIPanel.SetActive(false);
        currentLocker = null;
    }

    private void RefreshUI()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject itemPrefab = currentLocker.slotItems[i];
            
            slotButtons[i].onClick.RemoveAllListeners();

            if (itemPrefab != null)
            {
                slotIcons[i].gameObject.SetActive(true);
                
                SpriteRenderer sr = itemPrefab.GetComponent<SpriteRenderer>();
                if (sr != null) slotIcons[i].sprite = sr.sprite;

                int slotIndex = i;
                
                slotButtons[i].onClick.AddListener(() => LootItem(slotIndex));
            }
            else
            {
                slotIcons[i].gameObject.SetActive(false);
            }
        }
    }

    private void LootItem(int slotIndex)
    {
        GameObject itemPrefab = currentLocker.slotItems[slotIndex];
        if (itemPrefab == null) return;
        
        PickableItem pickable = itemPrefab.GetComponent<PickableItem>();

        if (pickable != null && pickable.itemData != null)
        {
            int amountToAdd = pickable.amount > 0 ? pickable.amount : 1;
            
            WeaponInstanceState weaponState = pickable.droppedWeaponState;
            
            bool itemAdded = InventoryManager.Instance.AddItem(pickable.itemData, amountToAdd, weaponState);

            if (itemAdded)
            {
                string itemID = currentLocker.GetSlotID(slotIndex);
                if (LootManager.Instance != null)
                {
                    LootManager.Instance.MarkAsLooted(itemID);
                }
                
                currentLocker.slotItems[slotIndex] = null;
                RefreshUI();
            }
            else
            {
                Debug.LogWarning("Brak miejsca w ekwipunku! Przedmiot zostaje w szafce.");
            }
        }
        else
        {
            Debug.LogError($"Prefab {itemPrefab.name} przypisany do szafki nie posiada komponentu PickableItem lub nie ma przypisanego ItemData!");
        }
    }
}
}