using UnityEngine;
using TMPro; // Wymagane dla TextMeshPro

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [Header("UI Components")]
    public GameObject tooltipObject;       // Panel tooltipa
    public TextMeshProUGUI nameText;       // Tekst nazwy
    public TextMeshProUGUI rarityText;     // Tekst rzadkości
    public TextMeshProUGUI descriptionText;// Tekst opisu

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
        // Ukryj dymek na starcie
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }

    private void Update()
    {
        if (tooltipObject != null && tooltipObject.activeSelf)
        {
            // Przesunięcie, żeby kursor nie zasłaniał tekstu
            Vector3 offset = new Vector3(115, -65, 0); 
            tooltipObject.transform.position = Input.mousePosition + offset;
        }
    }

    public void ShowTooltip(ItemData item)
    {
        if (item == null) return;

        tooltipObject.SetActive(true);
        tooltipObject.transform.SetAsLastSibling(); // Dymek zawsze na wierzchu

        nameText.text = item.itemName;
        descriptionText.text = item.description;
        rarityText.text = item.rarity.ToString();

        // --- TUTAJ BYŁ BŁĄD - TERAZ JEST POPRAWIONE ---
        // Dostosowałem kolory do Twoich rzadkości: Common, Rare, Unusual, Unique
        switch (item.rarity)
        {
            case ItemRarity.Common: 
                rarityText.color = Color.white; 
                nameText.color = Color.white;
                break;

            case ItemRarity.Rare: 
                rarityText.color = Color.cyan; // Jasnoniebieski
                nameText.color = Color.cyan;
                break;

            case ItemRarity.Unusual: 
                rarityText.color = Color.magenta; // Fioletowy
                nameText.color = Color.magenta;
                break;

            case ItemRarity.Unique: 
                rarityText.color = Color.yellow; // Złoty/Żółty
                nameText.color = Color.yellow;
                break;
        }
    }

    public void HideTooltip()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }
}