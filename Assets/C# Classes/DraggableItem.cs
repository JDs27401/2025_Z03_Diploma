using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag; // Tu zapamiętujemy, gdzie ma wylądować przedmiot
    public ItemData itemData; // Twoje dane przedmiotu

    private Image image;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Zapamiętujemy startowego rodzica (Ekwipunek)
        // Jeśli nie trafimy w żaden inny slot, to pozostanie on naszym celem powrotu
        parentAfterDrag = transform.parent;

        // 2. Wyciągamy ikonę na wierzch, żeby nie chowała się pod innymi slotami
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling();

        // 3. Pozwalamy myszce widzieć to, co jest POD ikoną
        image.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Używamy eventData.position (bezpieczne dla obu systemów Input)
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 4. Przywracamy blokowanie promieni
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;

        // 5. Ustawiamy rodzica
        // Jeśli trafiliśmy w slot -> parentAfterDrag to ten nowy slot (zmieniony przez CraftingSlot)
        // Jeśli NIE trafiliśmy -> parentAfterDrag to stary slot (z OnBeginDrag)
        transform.SetParent(parentAfterDrag);

        // 6. KLUCZOWE: Resetujemy pozycję do (0,0,0) względem nowego rodzica
        // Dzięki temu ikona "wskakuje" idealnie na środek slota
        transform.localPosition = Vector3.zero;
    }
}