using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance;

    [Header("UI Panels")]
    public GameObject journalPanel; 
    public Transform gridContainer; 
    public GameObject journalSlotPrefab; 

    [Header("Inspect Panel")]
    public GameObject inspectPanel; 
    public Image inspectImage; 

    [Header("Collectibles arrangement")]
    public ItemData[] allCollectibles = new ItemData[16];

    private HashSet<string> unlockedCollectibleIDs = new HashSet<string>();
    private List<JournalSlot> uiSlots = new List<JournalSlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        InitializeJournalUI();
        journalPanel.SetActive(false); 
        if(inspectPanel != null) inspectPanel.SetActive(false);
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

        Sprite imageToShow = item.fullSizeImage != null ? item.fullSizeImage : item.icon;

        if (imageToShow != null)
        {
            inspectImage.sprite = imageToShow;
            inspectImage.preserveAspect = true; 
            inspectPanel.SetActive(true);
        }
    }

    public void CloseInspectPanel()
    {
        if (inspectPanel != null) inspectPanel.SetActive(false);
    }
}