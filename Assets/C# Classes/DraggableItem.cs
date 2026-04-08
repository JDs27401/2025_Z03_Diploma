using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    // --- NOWA FLAGA STATYCZNA ---
    public static bool isDraggingItem = false; 

    [Header("UI Components")]
    public Image image;
    public TextMeshProUGUI amountText; 
    
    [Header("Item Data")]
    public ItemData itemData;
    public int count = 1;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public bool isSplitDrag = false; 
    
    private Transform startParent;
    private Transform rootCanvasTransform;
    private bool isHovered = false;

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
        UpdateTextPosition();
    }

    private void Update()
    {
        if (isHovered && Input.GetKeyDown(KeyCode.G))
        {
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.DropItem(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (itemData != null && TooltipManager.instance != null && !isDraggingItem)
        {
            TooltipManager.instance.ShowTooltip(itemData); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (TooltipManager.instance != null)
        {
            TooltipManager.instance.HideTooltip();
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

        if (TooltipManager.instance != null) TooltipManager.instance.HideTooltip();

        startParent = transform.parent;
        parentAfterDrag = transform.parent;
        isSplitDrag = false;

        InventorySlot currentSlot = startParent.GetComponent<InventorySlot>();
        if (currentSlot != null && currentSlot.lockContent)
        {
             eventData.pointerDrag = null;
             return;
        }

        // Ustawienie flagi na start przeciągania
        isDraggingItem = true;

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
        
        // Reset flagi po upuszczeniu
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

        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        if (slot != null)
        {
            DraggableItem[] items = parentAfterDrag.GetComponentsInChildren<DraggableItem>();
            if (items.Length > 1)
            {
                foreach (DraggableItem item in items)
                {
                    if (item != this && item.itemData == this.itemData)
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
        
        CraftingSlot cSlot = parentAfterDrag.GetComponent<CraftingSlot>();
        if (cSlot != null)
        {
            DraggableItem[] items = parentAfterDrag.GetComponentsInChildren<DraggableItem>();
            if (items.Length > 1)
            {
                foreach (DraggableItem item in items)
                {
                    if (item != this && item.itemData == this.itemData)
                    {
                        item.count += this.count;
                        item.RefreshCount(item.count);
                        cSlot.currentCount = item.count;
                        if (CraftingUI.instance != null) CraftingUI.instance.UpdateCraftingGrid();
                        Destroy(this.gameObject);
                        return;
                    }
                }
            }
            if (CraftingUI.instance != null) CraftingUI.instance.UpdateCraftingGrid();
        }
    }

    public void RefreshCount(int newCount)
    {
        count = newCount;
        if (amountText != null)
        {
            amountText.text = count.ToString();
            bool showText = false;

            if (itemData != null)
            {
                if (itemData.itemType != ItemType.General) showText = true;
                else if (count > 1) showText = true;
            }
            amountText.gameObject.SetActive(showText);
        }
    }

    // Dodatkowe zabezpieczenie: jeśli wyrzucimy przedmiot na ziemię podczas przeciągania, resetujemy flagę
    private void OnDisable()
    {
        if (isDraggingItem && image != null && !image.raycastTarget)
        {
            isDraggingItem = false;
        }
    }
}