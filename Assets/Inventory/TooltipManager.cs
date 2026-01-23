using UnityEngine;
using TMPro; // Wymagane dla TextMeshPro

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [Header("UI Components")]
    public GameObject tooltipObject;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI descriptionText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }

    private void Update()
    {
        if (tooltipObject != null && tooltipObject.activeSelf)
        {
            Vector3 offset = new Vector3(140, -50, 0); 
            tooltipObject.transform.position = Input.mousePosition + offset;
        }
    }

    public void ShowTooltip(ItemData item)
    {
        if (item == null) return;

        tooltipObject.SetActive(true);
        tooltipObject.transform.SetAsLastSibling();

        nameText.text = item.itemName;
        descriptionText.text = item.description;
        rarityText.text = item.rarity.ToString();

        switch (item.rarity)
        {
            case ItemRarity.Common: 
                // rarityText.color = Color.white; 
                nameText.color = new Color32(160, 160, 160, 255);
                break;

            case ItemRarity.Rare: 
                // rarityText.color = Color.cyan;
                nameText.color = new Color32(46, 73, 195, 255);
                break;

            case ItemRarity.Unusual: 
                // rarityText.color = Color.magenta;
                nameText.color = new Color32(185, 46, 195, 255);
                break;

            case ItemRarity.Unique: 
                // rarityText.color = Color.yellow;
                nameText.color = new Color32(195, 154, 46, 255);
                break;
        }
    }

    public void HideTooltip()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }
}