using C__Classes.Managers;
using Player.scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static bool isDraggingItem = false; 

    [Header("UI Components")]
    public Image image;
    public TextMeshProUGUI amountText; 
    
    [Header("Item Data")]
    public ItemData itemData;
    public int count = 1;
    public WeaponInstanceState weaponInstanceState;
    public MeleeWeaponInstanceState meleeInstanceState;

    [Header("Weapon Mod Attachment")]
    public bool isWeaponModAttachment = false;
    public WeaponModInstanceState weaponModInstanceState;
    public MeleeWeaponModInstanceState meleeWeaponModInstanceState;
    public DraggableItem attachedWeaponItemRoot;

    [Header("Weapon Mod Visuals")]
    public Sprite emptyModSlotSprite; 
    public int maxWeaponModSlots = 1; 
    public int maxMeleeWeaponModSlots = 1;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public bool isSplitDrag = false; 
    
    private Transform startParent;
    private Transform rootCanvasTransform;
    private bool isHovered = false;
    private bool weaponModTransferCommitted = false;

    private void Awake()
    {
        if (image == null) image = GetComponent<Image>();
        if (amountText == null) amountText = GetComponentInChildren<TextMeshProUGUI>();
        
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null) rootCanvasTransform = canvas.transform;
        else rootCanvasTransform = transform.root;
    }

    private void Start()
    {
        parentAfterDrag = transform.parent;
        EnsureWeaponStateInitialized();
        RefreshWeaponModVisuals();
        UpdateTextPosition();
    }

    public void EnsureWeaponStateInitialized()
    {
        if (itemData is WeaponItemData weaponItemData && weaponItemData.weaponData != null)
        {
            if (weaponInstanceState == null)
            {
                weaponInstanceState = new WeaponInstanceState(weaponItemData.weaponData.magazineSize);
            }
            else
            {
                weaponInstanceState.currentMagazineAmmo = Mathf.Clamp(weaponInstanceState.currentMagazineAmmo, 0, weaponItemData.weaponData.magazineSize);
            }

            weaponInstanceState.InitializeFromWeaponData(weaponItemData.weaponData);
            // clear melee state when this is a ranged weapon
            meleeInstanceState = null;
        }
        else if (itemData is MeleeWeaponItemData meleeItemData && meleeItemData.meleeWeaponData != null)
        {
            if (meleeInstanceState == null)
            {
                meleeInstanceState = new MeleeWeaponInstanceState();
            }

            meleeInstanceState.InitializeFromMeleeData(meleeItemData.meleeWeaponData);
            // clear ranged state when this is a melee weapon
            weaponInstanceState = null;
        }
        else
        {
            weaponInstanceState = null;
            meleeInstanceState = null;
        }
    }

    public bool IsWeaponModAttachment => isWeaponModAttachment && attachedWeaponItemRoot != null && (weaponModInstanceState != null || meleeWeaponModInstanceState != null);

    public void BindWeaponModAttachment(DraggableItem weaponItemRoot, WeaponModInstanceState modState)
    {
        attachedWeaponItemRoot = weaponItemRoot;
        weaponModInstanceState = modState;
        meleeWeaponModInstanceState = null;
        isWeaponModAttachment = true;
        weaponModTransferCommitted = false;

        if (modState != null && modState.itemData != null)
        {
            itemData = modState.itemData;
        }

        weaponInstanceState = null;
        RefreshWeaponModVisuals();
        UpdateTextPosition();
    }

    public void BindWeaponModAttachment(DraggableItem weaponItemRoot, MeleeWeaponModInstanceState modState)
    {
        attachedWeaponItemRoot = weaponItemRoot;
        meleeWeaponModInstanceState = modState;
        weaponModInstanceState = null;
        isWeaponModAttachment = true;
        weaponModTransferCommitted = false;

        if (modState != null && modState.itemData != null)
        {
            itemData = modState.itemData;
        }

        weaponInstanceState = null;
        meleeInstanceState = null;
        RefreshWeaponModVisuals();
        UpdateTextPosition();
    }

    public void RefreshWeaponModVisuals(DraggableItem excludedItem = null)
    {
        bool isRangedWeapon = itemData is WeaponItemData && weaponInstanceState != null;
        bool isMeleeWeapon = itemData is MeleeWeaponItemData && meleeInstanceState != null;
        if (itemData.itemType.Equals(ItemType.Molotov) || itemData.itemType.Equals(ItemType.Pipebomb))
        {
            return;
        }
        if (!isRangedWeapon && !isMeleeWeapon)
        {
            return;
        }

        DraggableItem[] childItems = GetComponentsInChildren<DraggableItem>(true);
        for (int i = 0; i < childItems.Length; i++)
        {
            if (childItems[i] != null && childItems[i] != this && childItems[i] != excludedItem)
            {
                Destroy(childItems[i].gameObject);
            }
        }

        if (InventoryManager.Instance == null || InventoryManager.Instance.inventoryItemPrefab == null)
        {
            return;
        }

        int installedCount;
        int totalSlotsToDraw;
        var rangedMods = weaponInstanceState != null ? weaponInstanceState.installedMods : null;
        var meleeMods = meleeInstanceState != null ? meleeInstanceState.installedMods : null;

        if (isRangedWeapon)
        {
            installedCount = rangedMods != null ? rangedMods.Count : 0;
            totalSlotsToDraw = Mathf.Max(installedCount, maxWeaponModSlots);
        }
        else
        {
            installedCount = meleeMods != null ? meleeMods.Count : 0;
            totalSlotsToDraw = Mathf.Max(installedCount, maxMeleeWeaponModSlots);
        }

        for (int i = 0; i < totalSlotsToDraw; i++)
        {
            GameObject modGo = Instantiate(InventoryManager.Instance.inventoryItemPrefab, transform);
            DraggableItem modItem = modGo.GetComponent<DraggableItem>();
            if (modItem == null)
            {
                continue;
            }

            RectTransform rect = modGo.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
                rect.sizeDelta = new Vector2(18f, 18f);
                rect.anchoredPosition = new Vector2(-6f - (i * 20f), -6f);
                rect.localScale = Vector3.one;
            }

            if (modItem.amountText != null)
            {
                modItem.amountText.gameObject.SetActive(false);
            }

            bool hasMod = i < installedCount;
            if (hasMod && isRangedWeapon)
            {
                WeaponModInstanceState modState = rangedMods[i];
                if (modState == null || modState.itemData == null) continue;

                modItem.itemData = modState.itemData;
                modItem.count = 1;
                modItem.weaponInstanceState = null;
                modItem.BindWeaponModAttachment(this, modState);

                if (modItem.image != null)
                {
                    Sprite icon = modState.itemData.icon;
                    if (icon == null && modState.modData != null)
                    {
                        icon = modState.modData.icon;
                    }

                    if (icon != null)
                    {
                        modItem.image.sprite = icon;
                        modItem.image.color = Color.white;
                    }
                }
            }
            else if (hasMod)
            {
                MeleeWeaponModInstanceState modState = meleeMods[i];
                if (modState == null || modState.itemData == null) continue;

                modItem.itemData = modState.itemData;
                modItem.count = 1;
                modItem.weaponInstanceState = null;
                modItem.meleeInstanceState = null;
                modItem.BindWeaponModAttachment(this, modState);

                if (modItem.image != null)
                {
                    Sprite icon = modState.itemData.icon;
                    if (icon == null && modState.modData != null)
                    {
                        icon = modState.modData.icon;
                    }

                    if (icon != null)
                    {
                        modItem.image.sprite = icon;
                        modItem.image.color = Color.white;
                    }
                }
            }
            else
            {
                modItem.enabled = false;
                if (modItem.image != null)
                {
                    modItem.image.sprite = emptyModSlotSprite;
                    modItem.image.color = new Color(1f, 1f, 1f, 0.6f);
                    modItem.image.raycastTarget = false;
                }
            }
        }
    }

    public void DetachWeaponModFromWeapon()
    {
        if (!IsWeaponModAttachment)
        {
            return;
        }

        if (attachedWeaponItemRoot != null && attachedWeaponItemRoot.weaponInstanceState != null)
        {
            attachedWeaponItemRoot.weaponInstanceState.RemoveMod(weaponModInstanceState);
            attachedWeaponItemRoot.RefreshWeaponModVisuals(this);
        }

        if (attachedWeaponItemRoot != null && attachedWeaponItemRoot.meleeInstanceState != null)
        {
            attachedWeaponItemRoot.meleeInstanceState.RemoveMod(meleeWeaponModInstanceState);
            attachedWeaponItemRoot.RefreshWeaponModVisuals(this);
        }
    }

    public void RestoreWeaponModToWeapon()
    {
        if (!IsWeaponModAttachment)
        {
            return;
        }

        if (attachedWeaponItemRoot != null && attachedWeaponItemRoot.weaponInstanceState != null)
        {
            attachedWeaponItemRoot.weaponInstanceState.RestoreMod(weaponModInstanceState);
            attachedWeaponItemRoot.RefreshWeaponModVisuals();
        }

        if (attachedWeaponItemRoot != null && attachedWeaponItemRoot.meleeInstanceState != null)
        {
            attachedWeaponItemRoot.meleeInstanceState.RestoreMod(meleeWeaponModInstanceState);
            attachedWeaponItemRoot.RefreshWeaponModVisuals();
        }
    }

    public void CommitWeaponModTransfer()
    {
        weaponModTransferCommitted = true;
        isWeaponModAttachment = false;
        attachedWeaponItemRoot = null;
        weaponModInstanceState = null;
        meleeWeaponModInstanceState = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.currentlyHoveredItem = this;
        }

        if (itemData != null && TooltipManager.Instance != null && !isDraggingItem)
        {
            TooltipManager.Instance.ShowTooltip(itemData); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        
        if (InventoryManager.Instance != null && InventoryManager.Instance.currentlyHoveredItem == this)
        {
            InventoryManager.Instance.currentlyHoveredItem = null;
        }

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    public void UpdateTextPosition()
    {
        if (itemData == null || amountText == null) return;
        RectTransform textRect = amountText.rectTransform;

        if (itemData.itemType == ItemType.Ammo9mm || itemData.itemType == ItemType.Ammo12Gauge)
        {
            if (ColorUtility.TryParseHtmlString("#585248", out Color ammoColor))
            {
                amountText.color = ammoColor;
            }
            
            textRect.anchorMin = new Vector2(1f, 0.5f); 
            textRect.anchorMax = new Vector2(1f, 0.5f);
            textRect.pivot = new Vector2(0f, 0.5f);
            textRect.anchoredPosition = new Vector2(10, 0);
            amountText.alignment = TextAlignmentOptions.Left;
        }
        else
        {
            amountText.color = Color.white;
            textRect.anchorMin = new Vector2(1, 0);
            textRect.anchorMax = new Vector2(1, 0);
            textRect.pivot = new Vector2(1, 0);
            textRect.anchoredPosition = new Vector2(-5, 0); 
            amountText.alignment = TextAlignmentOptions.BottomRight;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null) return;

        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();

        startParent = transform.parent;
        parentAfterDrag = transform.parent;
        isSplitDrag = false;

        InventorySlot currentSlot = startParent.GetComponent<InventorySlot>();
        if (currentSlot != null && currentSlot.lockContent)
        {
             eventData.pointerDrag = null;
             return;
        }

        isDraggingItem = true;

        if (IsWeaponModAttachment)
        {
            DetachWeaponModFromWeapon();
        }

        if (eventData.button == PointerEventData.InputButton.Right && count > 1)
        {
            isSplitDrag = true;
            
            GameObject clone = Instantiate(gameObject, startParent);
            DraggableItem cloneScript = clone.GetComponent<DraggableItem>();
            
            int remainingAmount = count - 1;
            
            cloneScript.count = remainingAmount;
            cloneScript.RefreshCount(remainingAmount);
            cloneScript.parentAfterDrag = startParent;
            cloneScript.isSplitDrag = false;
            // Clone whichever instance state is present (ranged or melee)
            cloneScript.weaponInstanceState = weaponInstanceState != null ? weaponInstanceState.Clone() : null;
            cloneScript.meleeInstanceState = meleeInstanceState != null ? meleeInstanceState.Clone() : null;
            
            if (currentSlot != null)
            {
                currentSlot.currentCount = remainingAmount;
                currentSlot.UpdateUI();
            }
            else
            {
                CraftingSlot cSlot = startParent.GetComponent<CraftingSlot>();
                if (cSlot != null) cSlot.currentCount = remainingAmount;
            }
            
            this.count = 1;
            this.RefreshCount(1);
        }

        transform.SetParent(rootCanvasTransform);
        transform.SetAsLastSibling();
        
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentAfterDrag == null || parentAfterDrag == rootCanvasTransform)
        {
            parentAfterDrag = startParent;
        }

        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        isSplitDrag = false; 
        
        isDraggingItem = false;
        
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localPosition = Vector3.zero; 
            rect.anchoredPosition = Vector2.zero; 
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        if (weaponModTransferCommitted)
        {
            isDraggingItem = false;
            image.raycastTarget = true;
            weaponModTransferCommitted = false;
            return;
        }

        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        if (slot != null)
        {
            DraggableItem[] items = parentAfterDrag.GetComponentsInChildren<DraggableItem>();
            if (items.Length > 1)
            {
                foreach (DraggableItem item in items)
                {
                    if (item == null || item == this || item.transform.parent != parentAfterDrag)
                    {
                        continue;
                    }

                    if (item.itemData == this.itemData && this.itemData != null && this.itemData.isStackable)
                    {
                        item.count += this.count;
                        item.RefreshCount(item.count);
                        slot.currentCount = item.count;
                        slot.UpdateUI();
                        Destroy(this.gameObject);
                        return;
                    }
                }
            }
            slot.UpdateUI();
        }

        if (IsWeaponModAttachment && parentAfterDrag != null && parentAfterDrag.GetComponent<InventorySlot>() != null)
        {
            isWeaponModAttachment = false;
            attachedWeaponItemRoot = null;
            weaponModInstanceState = null;
            meleeWeaponModInstanceState = null;
        }
        
        CraftingSlot cSlot = parentAfterDrag.GetComponent<CraftingSlot>();
        if (cSlot != null)
        {
            DraggableItem[] items = parentAfterDrag.GetComponentsInChildren<DraggableItem>();
            if (items.Length > 1)
            {
                foreach (DraggableItem item in items)
                {
                    if (item == null || item == this || item.transform.parent != parentAfterDrag)
                    {
                        continue;
                    }

                    if (item.itemData == this.itemData && this.itemData != null && this.itemData.isStackable)
                    {
                        item.count += this.count;
                        item.RefreshCount(item.count);
                        cSlot.currentCount = item.count;
                        if (CraftingUI.Instance != null) CraftingUI.Instance.UpdateCraftingGrid();
                        Destroy(this.gameObject);
                        return;
                    }
                }
            }
            if (CraftingUI.Instance != null) CraftingUI.Instance.UpdateCraftingGrid();
        }

        if (IsWeaponModAttachment && (slot == null && cSlot == null))
        {
            RestoreWeaponModToWeapon();
            transform.SetParent(startParent);
            parentAfterDrag = startParent;

            RectTransform restoreRect = GetComponent<RectTransform>();
            if (restoreRect != null)
            {
                restoreRect.localPosition = Vector3.zero;
                restoreRect.anchoredPosition = Vector2.zero;
                restoreRect.localScale = Vector3.one;
            }
        }
    }

    public void RefreshCount(int newCount)
    {
        print("refresh");
        count = newCount;
        EnsureWeaponStateInitialized();
        if (amountText != null)
        {
            amountText.text = count.ToString();
            bool showText = false;

            if (itemData != null)
            {
                switch(itemData.itemType)
                {
                    case ItemType.WeaponMod:
                    case ItemType.MeleeWeaponMod:
                        break;
                    case ItemType.Ammo9mm: 
                    case ItemType.Ammo12Gauge:
                        showText = true;
                        break;
                    default:
                        if (itemData.itemType != ItemType.General)
                        {
                            showText = true;
                        }
                        if (count > 1)
                        {
                            showText = true;
                        }
                        break;
                }
            }
            amountText.gameObject.SetActive(showText);
        }
    }

    private void OnDisable()
    {
        if (isDraggingItem && image != null && !image.raycastTarget)
        {
            isDraggingItem = false;
        }
    }
}