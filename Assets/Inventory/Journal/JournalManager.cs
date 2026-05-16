using System.Collections.Generic;
using C__Classes.Singletons;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace C__Classes.Managers
{
    public class JournalManager : SingletonNonPersistant<JournalManager>
    {
        [Header("UI Panels")]
        public GameObject journalPanel; 
        public Transform gridContainer; 
        public GameObject journalSlotPrefab; 

        [Header("Inspect Panel")]
        public GameObject inspectPanel; 
        public Image inspectImage; 

        [Header("Read Feature")]
        public GameObject descriptionPanel;
        public TextMeshProUGUI descriptionText;

        [Header("Collectibles arrangement")]
        public ItemData[] allCollectibles = new ItemData[16];

        private HashSet<string> unlockedCollectibleIDs = new HashSet<string>();
        private List<JournalSlot> uiSlots = new List<JournalSlot>();
        private ItemData currentlyInspectedItem;

        private void Start()
        {
            InitializeJournalUI();
            journalPanel.SetActive(false); 
            if(inspectPanel != null) inspectPanel.SetActive(false);
            if(descriptionPanel != null) descriptionPanel.SetActive(false);
            else if (descriptionText != null) descriptionText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (inspectPanel != null && inspectPanel.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    ToggleDescription();
                }
            }
        }

        private void InitializeJournalUI()
        {
            foreach (ItemData item in allCollectibles)
            {
                GameObject newSlotObj = Instantiate(journalSlotPrefab, gridContainer);
                JournalSlot slotScript = newSlotObj.GetComponent<JournalSlot>();
                
                if (slotScript != null)
                {
                    slotScript.Setup(item);
                    slotScript.SetUnlocked(false); 
                    uiSlots.Add(slotScript);
                }
            }
        }

        public void UnlockCollectible(string itemID)
        {
            if (!unlockedCollectibleIDs.Contains(itemID))
            {
                unlockedCollectibleIDs.Add(itemID);
                RefreshJournalUI();
            }
        }

        private void RefreshJournalUI()
        {
            for (int i = 0; i < allCollectibles.Length; i++)
            {
                if (allCollectibles[i] != null)
                {
                    bool isUnlocked = unlockedCollectibleIDs.Contains(allCollectibles[i].id);
                    uiSlots[i].SetUnlocked(isUnlocked);
                }
            }
        }

        public void ToggleJournal()
        {
            bool isActive = !journalPanel.activeSelf;
            journalPanel.SetActive(isActive);

            if (isActive) RefreshJournalUI();
        }

        public void CloseJournal()
        {
            journalPanel.SetActive(false);
        }

        public void ShowInspectPanel(ItemData item)
        {
            if (inspectPanel == null || inspectImage == null) return;

            currentlyInspectedItem = item;

            Sprite imageToShow = item.fullSizeImage != null ? item.fullSizeImage : item.icon;

            if (imageToShow != null)
            {
                inspectImage.sprite = imageToShow;
                inspectImage.preserveAspect = true; 
                inspectPanel.SetActive(true);
            }

            if (descriptionPanel != null) descriptionPanel.SetActive(false);
            else if (descriptionText != null) descriptionText.gameObject.SetActive(false);
        }

        public void CloseInspectPanel()
        {
            if (inspectPanel != null) inspectPanel.SetActive(false);
            if (descriptionPanel != null) descriptionPanel.SetActive(false);
            else if (descriptionText != null) descriptionText.gameObject.SetActive(false);
            
            currentlyInspectedItem = null;
        }

        private void ToggleDescription()
        {
            if (currentlyInspectedItem == null || descriptionText == null) return;

            bool isShowing = descriptionPanel != null ? !descriptionPanel.activeSelf : !descriptionText.gameObject.activeSelf;
            
            if (isShowing)
            {
                descriptionText.text = currentlyInspectedItem.description; 
            }

            if (descriptionPanel != null)
            {
                descriptionPanel.SetActive(isShowing);
            }
            else
            {
                descriptionText.gameObject.SetActive(isShowing);
            }
        }
    }
}