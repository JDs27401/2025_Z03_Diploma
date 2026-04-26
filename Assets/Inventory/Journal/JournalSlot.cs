using C__Classes.Managers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class JournalSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    
    private ItemData assignedItem;
    private bool isUnlocked = false;
    private Button slotButton; 

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(OnSlotClicked); 
    }

    public void Setup(ItemData item)
    {
        assignedItem = item;
        
        if (assignedItem != null && assignedItem.icon != null)
        {
            iconImage.sprite = assignedItem.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        UpdateVisuals();
    }

    public void SetUnlocked(bool state)
    {
        isUnlocked = state;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (iconImage == null || assignedItem == null) return;

        if (isUnlocked)
        {
            // Unlocked: full color, no outline
            iconImage.color = Color.white; 
        }
        else
        {
            // Locked: grayed out, white outline
            iconImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }

    private void OnSlotClicked()
    {
        if (isUnlocked && assignedItem != null)
        {
            JournalManager.Instance.ShowInspectPanel(assignedItem);
        }
    }
}