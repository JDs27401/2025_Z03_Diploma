using System.Collections.Generic;
using C__Classes.SaveSystem;
using C__Classes.Singletons;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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
        public CanvasGroup descriptionCanvasGroup;
        public float readFadeDuration = 0.5f;

        [Header("Collectibles arrangement")]
        public ItemData[] allCollectibles = new ItemData[16];

        [Header("Notification UI")]
        public GameObject notificationPanel;
        public TextMeshProUGUI notificationText;
        public CanvasGroup notificationCanvasGroup;
        public float notificationDuration = 3f;
        public float fadeDuration = 1f;

        private HashSet<string> unlockedCollectibleIDs = new HashSet<string>();
        private List<JournalSlot> uiSlots = new List<JournalSlot>();
        private ItemData currentlyInspectedItem;
        private bool isDescriptionOpen = false;
        private bool isInitialized = false;

        private void Start()
        {
            InitializeJournalUI();
            isInitialized = true;
            RefreshJournalUI();
            journalPanel.SetActive(false);
            if (inspectPanel != null) inspectPanel.SetActive(false);
            
            if (descriptionPanel != null)
            {
                descriptionPanel.SetActive(false);
                if (descriptionCanvasGroup != null) descriptionCanvasGroup.alpha = 0f;
            }
            else if (descriptionText != null)
            {
                descriptionText.gameObject.SetActive(false);
            }

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
                if (notificationCanvasGroup != null) notificationCanvasGroup.alpha = 0f;
            }
            
            isDescriptionOpen = false;
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
            if (string.IsNullOrWhiteSpace(itemID))
            {
                return;
            }

            if (!unlockedCollectibleIDs.Contains(itemID))
            {
                unlockedCollectibleIDs.Add(itemID);
                RefreshJournalUI();
            }
        }

        public JournalSaveData CaptureSaveData()
        {
            JournalSaveData saveData = new JournalSaveData();
            saveData.unlockedCollectibleIds.AddRange(unlockedCollectibleIDs);
            return saveData;
        }

        public void RestoreSaveData(JournalSaveData saveData)
        {
            unlockedCollectibleIDs.Clear();

            if (saveData != null && saveData.unlockedCollectibleIds != null)
            {
                for (int i = 0; i < saveData.unlockedCollectibleIds.Count; i++)
                {
                    string collectibleId = saveData.unlockedCollectibleIds[i];
                    if (!string.IsNullOrWhiteSpace(collectibleId))
                    {
                        unlockedCollectibleIDs.Add(collectibleId);
                    }
                }
            }

            RefreshJournalUI();
        }

        private void RefreshJournalUI()
        {
            if (!isInitialized || uiSlots.Count == 0)
            {
                return;
            }

            for (int i = 0; i < allCollectibles.Length; i++)
            {
                if (allCollectibles[i] != null && i < uiSlots.Count && uiSlots[i] != null)
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

            ResetDescriptionState();
        }

        public void CloseInspectPanel()
        {
            if (inspectPanel != null) inspectPanel.SetActive(false);
            ResetDescriptionState();
            currentlyInspectedItem = null;
        }

        private void ResetDescriptionState()
        {
            isDescriptionOpen = false;
            if (descriptionCanvasGroup != null) descriptionCanvasGroup.DOKill();
            if (descriptionPanel != null)
            {
                descriptionPanel.SetActive(false);
                if (descriptionCanvasGroup != null) descriptionCanvasGroup.alpha = 0f;
            }
            else if (descriptionText != null)
            {
                descriptionText.gameObject.SetActive(false);
            }
        }

        private void ToggleDescription()
        {
            if (currentlyInspectedItem == null || descriptionText == null) return;

            if (descriptionCanvasGroup == null || descriptionPanel == null)
            {
                bool isShowing = descriptionPanel != null ? !descriptionPanel.activeSelf : !descriptionText.gameObject.activeSelf;
                if (isShowing) descriptionText.text = currentlyInspectedItem.description;
                if (descriptionPanel != null) descriptionPanel.SetActive(isShowing);
                else descriptionText.gameObject.SetActive(isShowing);
                return;
            }

            isDescriptionOpen = !isDescriptionOpen;
            descriptionCanvasGroup.DOKill();

            if (isDescriptionOpen)
            {
                descriptionText.text = currentlyInspectedItem.description;
                descriptionPanel.SetActive(true);
                descriptionCanvasGroup.DOFade(1f, readFadeDuration);
            }
            else
            {
                descriptionCanvasGroup.DOFade(0f, readFadeDuration)
                    .OnComplete(() => descriptionPanel.SetActive(false));
            }
        }

        public void ShowNotification(string itemName)
        {
            ShowMessage($"New Journal entry: {itemName}");
        }

        public void ShowMessage(string message)
        {
            if (notificationPanel == null || notificationText == null || notificationCanvasGroup == null) return;

            notificationText.text = message;

            notificationCanvasGroup.DOKill();
            notificationCanvasGroup.alpha = 1f;
            notificationPanel.SetActive(true);

            notificationCanvasGroup.DOFade(0f, fadeDuration)
                .SetDelay(notificationDuration)
                .OnComplete(() => notificationPanel.SetActive(false));
        }
    }
}
