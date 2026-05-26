using C__Classes.Singletons;
using UnityEngine;
using TMPro;

namespace C__Classes.Managers
{
    public class TooltipManager : SingletonNonPersistant<TooltipManager>
    {
        [Header("UI Components")]
        public GameObject tooltipObject;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI descriptionText;
        public GameObject hintsContainer;

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

        public void ShowTooltip(ItemData item, bool isDiscovered = true, bool showHints = true) 
        {
            if (item == null) return;

            tooltipObject.SetActive(true);
            tooltipObject.transform.SetAsLastSibling();

            if (hintsContainer != null)
            {
                hintsContainer.SetActive(showHints);
            }

            if (isDiscovered)
            {
                nameText.text = item.itemName;
                descriptionText.text = item.description;
            }
            else
            {
                nameText.text = "?????";
                descriptionText.text = "?????";
            }

            rarityText.text = item.rarity.ToString();

            switch (item.rarity)
            {
                case ItemRarity.Common: 
                    nameText.color = new Color32(160, 160, 160, 255);
                    break;
                case ItemRarity.Rare: 
                    nameText.color = new Color32(46, 73, 195, 255);
                    break;
                case ItemRarity.Unusual: 
                    nameText.color = new Color32(185, 46, 195, 255);
                    break;
                case ItemRarity.Unique: 
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
}