using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using C__Classes.SaveSystem;
using C__Classes.Singletons;
using Player.scripts;
using Unity.VisualScripting;


namespace C__Classes.Managers
{
    public class InventoryManager : SingletonNonPersistant<InventoryManager>
    {
        [Header("Configuration")]
        public InventorySlot[] inventorySlots;
        public GameObject inventoryItemPrefab;
        [Header("UI")]
        [SerializeField] private TMPro.TMP_FontAsset consumableEffectsFont;
        
        [Header("Hotbar Settings")]
        public int hotbarSlotsCount = 4;
        public int selectedSlotIndex = 0;
        
        [HideInInspector] public DraggableItem currentlyHoveredItem;

        private readonly ConsumableEffectManager consumableEffectManager = new ConsumableEffectManager();
        private ConsumableEffectsHUD consumableEffectsHUD;
        // Forwarded event when consumable effects change
        public event System.Action OnConsumableEffectsChanged;

        private void Start()
        {
            SelectSlot(selectedSlotIndex);
            InitializeConsumableEffectsHud();
            // Forward events from effect manager
            consumableEffectManager.OnEffectsChanged += () => OnConsumableEffectsChanged?.Invoke();
        }
        
        private void Update()
        {
            if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
            consumableEffectManager.Tick(Time.deltaTime);
            HandleHotbarInput();
            HandleDropInput();
        }

        public void InitializeStaticSlot(InventorySlot slot, ItemData item)
        {
            SpawnNewItemInSlot(slot, item, 0);

            if (slot.iconDisplay != null)
            {
                slot.iconDisplay.raycastTarget = false; 
            }
        }

        public bool AddItem(ItemData item, int amount = 1)
        {
            return AddItem(item, amount, null, null);
        }

        public bool AddItem(ItemData item, int amount, WeaponInstanceState weaponState)
        {
            return AddItem(item, amount, weaponState, null);
        }

        public bool AddItem(ItemData item, int amount, WeaponInstanceState weaponState, MeleeWeaponInstanceState meleeState)
        {
            if (item.isStackable)
            {
                foreach (InventorySlot slot in inventorySlots)
                {
                    if (slot.currentItem == item)
                    {
                        if (slot.currentCount < item.maxStackSize)
                        {
                            int spaceInStack = item.maxStackSize - slot.currentCount;
                            int amountToAdd = Mathf.Min(spaceInStack, amount);

                            slot.currentCount += amountToAdd;
                            
                            if (slot.iconDisplay != null) slot.iconDisplay.color = Color.white;

                            slot.UpdateUI();

                            amount -= amountToAdd;
                            
                            if (amount <= 0) 
                            {
                                return true; 
                            }
                        }
                    }
                }
            }

            while (amount > 0)
            {
                InventorySlot emptySlot = FindFirstFreeSlot(item.itemType);

                if (emptySlot != null)
                {
                    int amountToAdd = item.isStackable ? Mathf.Min(item.maxStackSize, amount) : 1;
                    
                    WeaponInstanceState stateForNewItem = weaponState != null ? weaponState.Clone() : null;
                    MeleeWeaponInstanceState meleeStateForNewItem = meleeState != null ? meleeState.Clone() : null;
                    SpawnNewItemInSlot(emptySlot, item, amountToAdd, stateForNewItem, meleeStateForNewItem);
                    
                    amount -= amountToAdd;
                }
                else
                {
                    return false; 
                }
            }

            return true;
        }

        public List<InventorySlotSaveData> CaptureInventorySaveData()
        {
            List<InventorySlotSaveData> result = new List<InventorySlotSaveData>();
            if (inventorySlots == null)
            {
                return result;
            }

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];
                if (slot == null || slot.currentItem == null || slot.currentCount <= 0)
                {
                    continue;
                }

                DraggableItem draggableItem = slot.GetMainDraggableItem();
                InventorySlotSaveData slotSaveData = new InventorySlotSaveData
                {
                    slotIndex = i,
                    itemId = slot.currentItem.id,
                    count = slot.currentCount
                };

                if (draggableItem != null && draggableItem.weaponInstanceState != null)
                {
                    slotSaveData.weaponState = SaveStateMapper.CaptureWeaponState(draggableItem.weaponInstanceState);
                }

                if (draggableItem != null && draggableItem.meleeInstanceState != null)
                {
                    slotSaveData.meleeWeaponState = SaveStateMapper.CaptureMeleeWeaponState(draggableItem.meleeInstanceState);
                }

                result.Add(slotSaveData);
            }

            return result;
        }

        public void RestoreInventoryFromSaveData(IReadOnlyList<InventorySlotSaveData> saveSlots, ItemDatabase itemDatabase)
        {
            if (inventorySlots == null || itemDatabase == null)
            {
                return;
            }

            ClearInventoryForLoad();

            if (saveSlots == null)
            {
                return;
            }

            for (int i = 0; i < saveSlots.Count; i++)
            {
                InventorySlotSaveData slotSaveData = saveSlots[i];
                if (slotSaveData == null || slotSaveData.slotIndex < 0 || slotSaveData.slotIndex >= inventorySlots.Length)
                {
                    continue;
                }

                ItemData item = itemDatabase.GetItemById(slotSaveData.itemId);
                if (item == null)
                {
                    Debug.LogWarning($"Could not load inventory item with id '{slotSaveData.itemId}'.");
                    continue;
                }

                WeaponInstanceState weaponState = SaveStateMapper.RestoreWeaponState(slotSaveData.weaponState, item, itemDatabase);
                MeleeWeaponInstanceState meleeState = SaveStateMapper.RestoreMeleeWeaponState(slotSaveData.meleeWeaponState, item, itemDatabase);
                SpawnNewItemInSlot(inventorySlots[slotSaveData.slotIndex], item, slotSaveData.count, weaponState, meleeState);
            }

            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, Mathf.Min(hotbarSlotsCount - 1, inventorySlots.Length - 1));
            SelectSlot(selectedSlotIndex);

            if (C__Classes.Systems.PlayerNoiseSystem.Instance != null)
            {
                C__Classes.Systems.PlayerNoiseSystem.Instance.UpdateNoiseRadius();
            }
        }

        private void ClearInventoryForLoad()
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];
                if (slot == null)
                {
                    continue;
                }

                DraggableItem[] childItems = slot.GetComponentsInChildren<DraggableItem>(true);
                for (int childIndex = 0; childIndex < childItems.Length; childIndex++)
                {
                    if (childItems[childIndex] != null)
                    {
                        Destroy(childItems[childIndex].gameObject);
                    }
                }

                slot.ClearSlot();
            }
        }

        public int GetTotalItemCount(ItemType itemType)
        {
            if (inventorySlots == null)
            {
                return 0;
            }

            int totalCount = 0;
            foreach (InventorySlot inventorySlot in inventorySlots)
            {
                if (inventorySlot == null || inventorySlot.currentItem == null || inventorySlot.currentItem.itemType != itemType || inventorySlot.currentCount <= 0)
                {
                    continue;
                }

                totalCount += inventorySlot.currentCount;
            }

            return totalCount;
        }

        public void ConsumeItemCount(ItemType itemType, int amount)
        {
            if (amount <= 0 || inventorySlots == null)
            {
                return;
            }

            int remaining = amount;

            foreach (InventorySlot inventorySlot in inventorySlots)
            {
                if (inventorySlot == null || inventorySlot.currentItem == null || inventorySlot.currentItem.itemType != itemType || inventorySlot.currentCount <= 0)
                {
                    continue;
                }

                int consumedFromSlot = Mathf.Min(remaining, inventorySlot.currentCount);
                inventorySlot.currentCount -= consumedFromSlot;
                remaining -= consumedFromSlot;

                if (inventorySlot.currentCount <= 0)
                {
                    if (inventorySlot.lockContent)
                    {
                        inventorySlot.currentCount = 0;
                        inventorySlot.UpdateUI();
                    }
                    else
                    {
                        if (TooltipManager.Instance != null)
                        {
                            TooltipManager.Instance.HideTooltip();
                        }

                        if (inventorySlot != null)
                        {
                            inventorySlot.ClearSlot();
                        }
                        
                        Object.Destroy(inventorySlot.GetComponentInChildren<DraggableItem>().gameObject);
                    }
                }
                else
                {
                    inventorySlot.UpdateUI();
                }

                if (remaining <= 0)
                {
                    break;
                }
            }
        }

        private void SpawnNewItemInSlot(InventorySlot slot, ItemData item, int amount, WeaponInstanceState weaponState = null, MeleeWeaponInstanceState meleeState = null)
        {
            GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
            
            RectTransform rect = newItemGo.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.localScale = Vector3.one;
                rect.anchorMin = Vector2.zero; 
                rect.anchorMax = Vector2.one;  

                rect.offsetMin = Vector2.zero; 
                rect.offsetMax = Vector2.zero; 
            }

            DraggableItem dragItem = newItemGo.GetComponent<DraggableItem>();
            if (dragItem != null)
            {
                dragItem.itemData = item;
                dragItem.count = amount;
                dragItem.weaponInstanceState = weaponState != null ? weaponState.Clone() : null;
                dragItem.meleeInstanceState = meleeState != null ? meleeState.Clone() : null;
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

            Image image = newItemGo.GetComponent<Image>();
            if (image == null) image = newItemGo.GetComponentInChildren<Image>();

            if (image != null)
            {
                if (item.icon != null)
                {
                    image.sprite = item.icon;
                    image.color = Color.white; 
                    image.enabled = true;
                }
            }

            slot.currentItem = item;
            slot.currentCount = amount;
            slot.iconDisplay = image;
            
            slot.UpdateUI();
        }

        private InventorySlot FindFirstFreeSlot(ItemType typeToCheck)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.currentItem == null)
                {
                    if (slot.allowedType == typeToCheck && slot.allowedType != ItemType.General)
                    {
                        return slot;
                    }
                }
            }

            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.currentItem == null)
                {
                    if (slot.allowedType == ItemType.General)
                    {
                        return slot;
                    }
                }
            }

            return null;
        }

        public void DropItem(DraggableItem draggableItem)
        {
            if (draggableItem == null || draggableItem.itemData == null || draggableItem.itemData.dropPrefab == null)
            {
                return;
            }

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            if (player == null)
            {
                return;
            }

            Vector3 dropOffset = new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(-0.4f, 0.4f), 0);
            Vector3 dropPosition = player.transform.position + dropOffset;
            dropPosition.z = 0f; 

            GameObject droppedObj = Instantiate(draggableItem.itemData.dropPrefab);
            droppedObj.transform.position = dropPosition;
            droppedObj.transform.rotation = Quaternion.identity;
            droppedObj.name = draggableItem.itemData.itemName + " (Dropped)";
            
            C__Classes.Systems.LootTracker lootTracker = droppedObj.GetComponent<C__Classes.Systems.LootTracker>();
            if (lootTracker != null)
            {
                lootTracker.isDynamicDrop = true;
            }

            droppedObj.SetActive(true);

            PickableItem pickable = droppedObj.GetComponent<PickableItem>();
            if (pickable != null)
            {
                pickable.amount = 1;
                pickable.itemData = draggableItem.itemData;
                pickable.SetDroppedStates(draggableItem.weaponInstanceState, draggableItem.meleeInstanceState);
            }

            Transform targetParent = draggableItem.parentAfterDrag != null ? draggableItem.parentAfterDrag : draggableItem.transform.parent;
            InventorySlot slot = targetParent.GetComponent<InventorySlot>();
            
            draggableItem.count--;

            if (slot != null)
            {
                slot.currentCount = draggableItem.count;
            }

            if (draggableItem.count <= 0)
            {
                if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
                
                if (slot != null) slot.ClearSlot();
                Destroy(draggableItem.gameObject);
            }
            else
            {
                draggableItem.RefreshCount(draggableItem.count);
                if (slot != null) slot.UpdateUI();
            }

            if (C__Classes.Systems.PlayerNoiseSystem.Instance != null)
            {
                C__Classes.Systems.PlayerNoiseSystem.Instance.UpdateNoiseRadius();
            }
        }

        public float GetTotalWeight()
        {
            float totalWeight = 0f;
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.currentItem == null) continue;

                float itemWeight = slot.currentItem.weight;

                // If the item is a weapon, prefer the runtime weight stored in its WeaponInstanceState
                DraggableItem draggable = slot.GetComponentInChildren<DraggableItem>();
                if (draggable != null && draggable.itemData is WeaponItemData)
                {
                    if (draggable.weaponInstanceState != null)
                    {
                        WeaponRuntimeStats runtime = draggable.weaponInstanceState.GetRuntimeStats();
                        if (runtime != null)
                        {
                            itemWeight = runtime.weight;
                        }
                    }
                }
                else if (draggable != null && draggable.itemData is MeleeWeaponItemData)
                {
                    if (draggable.meleeInstanceState != null)
                    {
                        MeleeWeaponRuntimeStats runtime = draggable.meleeInstanceState.GetRuntimeStats();
                        if (runtime != null)
                        {
                            itemWeight = runtime.weight;
                        }
                    }
                }

                totalWeight += itemWeight * slot.currentCount;
            }

            return totalWeight;
        }
        
        private void HandleHotbarInput()
        {
            int previousSelected = selectedSlotIndex;

            if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlotIndex = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlotIndex = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlotIndex = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlotIndex = 3;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                selectedSlotIndex--;
                if (selectedSlotIndex < 0) selectedSlotIndex = hotbarSlotsCount - 1;
            }
            else if (scroll < 0f)
            {
                selectedSlotIndex++;
                if (selectedSlotIndex >= hotbarSlotsCount) selectedSlotIndex = 0;
            }

            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, Mathf.Min(hotbarSlotsCount - 1, inventorySlots.Length - 1));

            if (previousSelected != selectedSlotIndex)
            {
                SelectSlot(selectedSlotIndex);
            }
        }
        
        private void SelectSlot(int index)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].SetSelected(i == index);
            }
        }

        public ItemData GetActiveItem()
        {
            if (inventorySlots.Length > selectedSlotIndex)
            {
                return inventorySlots[selectedSlotIndex].currentItem;
            }
            return null;
        }

        public DraggableItem GetActiveDraggableItem()
        {
            if (inventorySlots.Length > selectedSlotIndex)
            {
                return inventorySlots[selectedSlotIndex].GetComponentInChildren<DraggableItem>();
            }

            return null;
        }

        public IReadOnlyList<ConsumableEffectInstance> GetActiveConsumableEffects()
        {
            return consumableEffectManager.GetActiveEffectsSnapshot();
        }

        public float GetConsumableSpeedMultiplier()
        {
            return consumableEffectManager.GetSpeedMultiplier();
        }

        public float GetConsumableAccelerationMultiplier()
        {
            return consumableEffectManager.GetAccelerationMultiplier();
        }

        public float GetConsumableMaxHealthBonus()
        {
            return consumableEffectManager.GetMaxHealthBonus();
        }

        public float GetConsumableHealthPerSecond()
        {
            return consumableEffectManager.GetHealthPerSecond();
        }

        // New passthroughs for stamina / damage / noise / weapon spread
        public float GetConsumableMaxStaminaBonus()
        {
            return consumableEffectManager.GetMaxStaminaBonus();
        }

        public float GetConsumableStaminaPerSecond()
        {
            return consumableEffectManager.GetStaminaPerSecond();
        }

        public float GetConsumableDamageTakenMultiplier()
        {
            return consumableEffectManager.GetDamageTakenMultiplier();
        }

        public float GetConsumableNoiseMultiplier()
        {
            return consumableEffectManager.GetNoiseMultiplier();
        }

        public float GetConsumableWeaponSpreadMultiplier()
        {
            return consumableEffectManager.GetWeaponSpreadMultiplier();
        }

        public bool TryUseActiveConsumable()
        {
            if (inventorySlots == null || selectedSlotIndex < 0 || selectedSlotIndex >= inventorySlots.Length)
            {
                return false;
            }

            InventorySlot activeSlot = inventorySlots[selectedSlotIndex];
            if (activeSlot == null)
            {
                return false;
            }

            DraggableItem activeItem = activeSlot.GetComponentInChildren<DraggableItem>();
            if (activeItem == null || !(activeItem.itemData is ConsumableItemData consumableItemData))
            {
                return false;
            }

            if (consumableItemData.effects == null || consumableItemData.effects.Count == 0)
            {
                return false;
            }

            bool applied = consumableEffectManager.ApplyConsumable(consumableItemData);
            if (!applied)
            {
                return false;
            }

            ConsumeOneFromActiveSlot(activeItem, activeSlot);
            return true;
        }

        private void ConsumeOneFromActiveSlot(DraggableItem item, InventorySlot slot)
        {
            if (item == null)
            {
                return;
            }

            item.count--;

            if (slot != null)
            {
                slot.currentCount = item.count;
            }

            if (item.count <= 0)
            {
                if (TooltipManager.Instance != null)
                {
                    TooltipManager.Instance.HideTooltip();
                }

                if (slot != null)
                {
                    slot.ClearSlot();
                }

                Object.Destroy(item.gameObject);
                return;
            }

            item.RefreshCount(item.count);
            if (slot != null)
            {
                slot.UpdateUI();
            }
        }

        private void InitializeConsumableEffectsHud()
        {
            if (consumableEffectsHUD != null)
            {
                return;
            }

            GameObject hudRoot = new GameObject("ConsumableEffectsHUD", typeof(RectTransform));
            Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();
            if (existingCanvas != null)
            {
                hudRoot.transform.SetParent(existingCanvas.transform, false);
            }
            else
            {
                Canvas canvas = hudRoot.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000;
                hudRoot.AddComponent<CanvasScaler>();
                hudRoot.AddComponent<GraphicRaycaster>();
            }

            consumableEffectsHUD = hudRoot.AddComponent<ConsumableEffectsHUD>();
            consumableEffectsHUD.Initialize(consumableEffectManager);

            // Assign font: prefer explicit inspector assignment, otherwise try to auto-find by name
            if (consumableEffectsFont != null)
            {
                consumableEffectsHUD.FontAsset = consumableEffectsFont;
            }
            else
            {
                try
                {
                    var allFonts = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>();
                    for (int i = 0; i < allFonts.Length; i++)
                    {
                        if (allFonts[i] != null && allFonts[i].name == "04B_03__ SDF")
                        {
                            consumableEffectsHUD.FontAsset = allFonts[i];
                            break;
                        }
                    }
                }
                catch { }
            }
        }
        
        private void HandleDropInput()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (currentlyHoveredItem != null)
                {
                    DropItem(currentlyHoveredItem);
                }
                else
                {
                    DropActiveSlotItem();
                }
            }
        }

        private void DropActiveSlotItem()
        {
            if (inventorySlots.Length > selectedSlotIndex)
            {
                InventorySlot activeSlot = inventorySlots[selectedSlotIndex];
                
                DraggableItem itemInSlot = activeSlot.GetComponentInChildren<DraggableItem>();
                
                if (itemInSlot != null)
                {
                    DropItem(itemInSlot);
                }
            }
        }
    }
}
