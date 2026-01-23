using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image image;
    public TextMeshProUGUI amountText; 
    
    [Header("Item Data")]
    public ItemData itemData;
    public int count = 1;

    [HideInInspector] public Transform parentAfterDrag;
    
    private Transform startParent;
    private Transform rootCanvasTransform;

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
        UpdateTextPosition();
    }

// Tooltip handler
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null && TooltipManager.instance != null)
        {
            TooltipManager.instance.ShowTooltip(itemData); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.instance != null)
        {
            TooltipManager.instance.HideTooltip();
        }
    }

// Count text position updater
    public void UpdateTextPosition()
    {
        if (itemData == null || amountText == null) return;
        RectTransform textRect = amountText.rectTransform;

        // Check for ammo types to adjust text position and color
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

    // Drag & Drop handlers
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null) return;

        if (TooltipManager.instance != null) TooltipManager.instance.HideTooltip();

        startParent = transform.parent;
        parentAfterDrag = transform.parent;

        InventorySlot currentSlot = startParent.GetComponent<InventorySlot>();
        if (currentSlot != null && currentSlot.lockContent)
        {
             eventData.pointerDrag = null;
             return;
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
        if (slot != null) slot.UpdateUI();
        
        CraftingSlot cSlot = parentAfterDrag.GetComponent<CraftingSlot>();
        if (cSlot != null && CraftingUI.instance != null) CraftingUI.instance.UpdateCraftingGrid();
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
}