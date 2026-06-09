using C__Classes.Managers;
using C__Classes.SaveSystem;
using C__Classes.Systems;
using UnityEngine;

public class LockerUIManager : MonoBehaviour
{
    public static LockerUIManager Instance { get; private set; }

    [Header("Elementy UI")]
    public GameObject lockerUIPanel;

    public InventorySlot[] lockerSlots;

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
        if (currentLocker != null)
        {
            for (int i = 0; i < lockerSlots.Length; i++)
            {
                currentLocker.SetSlotState(i, CaptureLockerUiSlot(i));

                if (lockerSlots[i].currentItem == null && LootManager.Instance != null)
                {
                    LootManager.Instance.MarkAsLooted(currentLocker.GetSlotID(i));
                }
            }

            currentLocker.SaveCurrentState();
            ClearLockerUiSlots();
        }

        lockerUIPanel.SetActive(false);
        currentLocker = null;
    }

    private void RefreshUI()
    {
        ClearLockerUiSlots();

        for (int i = 0; i < lockerSlots.Length; i++)
        {
            ContainerSlotSaveData slotState = currentLocker.GetSlotState(i);
            ItemData itemData = null;
            int count = 0;
            Player.scripts.WeaponInstanceState weaponState = null;
            Player.scripts.MeleeWeaponInstanceState meleeState = null;

            if (slotState != null && !string.IsNullOrWhiteSpace(slotState.itemId) && SaveGameManager.ActiveItemDatabase != null)
            {
                itemData = SaveGameManager.ActiveItemDatabase.GetItemById(slotState.itemId);
                count = slotState.count;
                weaponState = SaveStateMapper.RestoreWeaponState(slotState.weaponState, itemData, SaveGameManager.ActiveItemDatabase);
                meleeState = SaveStateMapper.RestoreMeleeWeaponState(slotState.meleeWeaponState, itemData, SaveGameManager.ActiveItemDatabase);
            }
            else if (i < currentLocker.slotItems.Length && currentLocker.slotItems[i] != null)
            {
                PickableItem pickable = currentLocker.slotItems[i].GetComponent<PickableItem>();
                if (pickable != null && pickable.itemData != null)
                {
                    itemData = pickable.itemData;
                    count = pickable.amount > 0 ? pickable.amount : 1;
                    weaponState = pickable.GetDroppedWeaponStateClone();
                    meleeState = pickable.GetDroppedMeleeStateClone();
                }
            }

            if (itemData != null)
            {
                SpawnUiItem(i, itemData, count, weaponState, meleeState);
            }
        }
    }

    private void SpawnUiItem(
        int slotIndex,
        ItemData itemData,
        int count,
        Player.scripts.WeaponInstanceState weaponState,
        Player.scripts.MeleeWeaponInstanceState meleeState)
    {
        GameObject newItemGo = Instantiate(InventoryManager.Instance.inventoryItemPrefab, lockerSlots[slotIndex].transform);

        RectTransform rect = newItemGo.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        DraggableItem dragItem = newItemGo.GetComponent<DraggableItem>();
        if (dragItem != null)
        {
            dragItem.itemData = itemData;
            dragItem.count = count > 0 ? count : 1;
            dragItem.weaponInstanceState = weaponState;
            dragItem.meleeInstanceState = meleeState;
            dragItem.EnsureWeaponStateInitialized();

            if (weaponState != null)
            {
                dragItem.weaponInstanceState = weaponState.Clone();
                dragItem.meleeInstanceState = null;
            }

            if (meleeState != null)
            {
                dragItem.weaponInstanceState = null;
                dragItem.meleeInstanceState = meleeState.Clone();
            }

            dragItem.RefreshWeaponModVisuals();
        }

        UnityEngine.UI.Image image = newItemGo.GetComponent<UnityEngine.UI.Image>();
        if (image == null) image = newItemGo.GetComponentInChildren<UnityEngine.UI.Image>();

        if (image != null && itemData.icon != null)
        {
            image.sprite = itemData.icon;
            image.color = Color.white;
            image.enabled = true;
        }

        lockerSlots[slotIndex].currentItem = itemData;
        lockerSlots[slotIndex].currentCount = dragItem != null ? dragItem.count : count;
        lockerSlots[slotIndex].iconDisplay = image;
        lockerSlots[slotIndex].UpdateUI();
    }

    private ContainerSlotSaveData CaptureLockerUiSlot(int slotIndex)
    {
        InventorySlot slot = lockerSlots[slotIndex];
        ContainerSlotSaveData slotSaveData = new ContainerSlotSaveData
        {
            slotIndex = slotIndex,
            itemId = null,
            count = 0
        };

        if (slot == null || slot.currentItem == null || slot.currentCount <= 0)
        {
            return slotSaveData;
        }

        DraggableItem draggableItem = slot.GetMainDraggableItem();
        slotSaveData.itemId = slot.currentItem.id;
        slotSaveData.count = slot.currentCount;
        slotSaveData.weaponState = draggableItem != null ? SaveStateMapper.CaptureWeaponState(draggableItem.weaponInstanceState) : null;
        slotSaveData.meleeWeaponState = draggableItem != null ? SaveStateMapper.CaptureMeleeWeaponState(draggableItem.meleeInstanceState) : null;
        return slotSaveData;
    }

    private void ClearLockerUiSlots()
    {
        for (int i = 0; i < lockerSlots.Length; i++)
        {
            lockerSlots[i].ClearSlot();
            foreach (Transform child in lockerSlots[i].transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
