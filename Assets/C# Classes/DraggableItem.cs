using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
// DODALIŚMY: IPointerEnterHandler, IPointerExitHandler
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public ItemData itemData; // Dane przedmiotu
    
    private Image image;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // --- NOWA LOGIKA TOOLTIPA (W PRZEDMIOCIE) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Kiedy najeżdżasz na sam przedmiot -> Pokaż Tooltip
        if (itemData != null)
        {
            TooltipManager.instance.ShowTooltip(itemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kiedy zjeżdżasz z przedmiotu -> Ukryj Tooltip
        TooltipManager.instance.HideTooltip();
    }
    // --------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Ukrywamy tooltip od razu jak zaczynamy ciągnąć
        TooltipManager.instance.HideTooltip();

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling();

        image.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        
        // Fix skali po upuszczeniu (na wszelki wypadek)
        transform.localScale = Vector3.one; 
    }
}