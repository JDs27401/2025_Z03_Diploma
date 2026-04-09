using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance;

    [Header("UI Panels")]
    public GameObject journalPanel; // Główny panel dziennika (ten, który się włącza/wyłącza)
    public Transform gridContainer; // Obiekt z komponentem Grid Layout Group
    public GameObject journalSlotPrefab; // Prefab pojedynczego slota (z podpiętym JournalSlot.cs)

    [Header("Baza Znajdziek (Ustal kolejność!)")]
    [Tooltip("Dodaj tu 16 przedmiotów: 1-8 notatki, 9-12 gazety, 13-16 plakaty")]
    public ItemData[] allCollectibles = new ItemData[16];

    // Lista przechowująca ID odblokowanych przedmiotów
    private HashSet<string> unlockedCollectibleIDs = new HashSet<string>();
    
    // Lista fizycznych slotów w UI, aby łatwo je aktualizować
    private List<JournalSlot> uiSlots = new List<JournalSlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        InitializeJournalUI();
        journalPanel.SetActive(false); // Dziennik domyślnie ukryty
    }

    private void InitializeJournalUI()
    {
        // Tworzenie 16 slotów na podstawie bazy danych
        foreach (ItemData item in allCollectibles)
        {
            GameObject newSlotObj = Instantiate(journalSlotPrefab, gridContainer);
            JournalSlot slotScript = newSlotObj.GetComponent<JournalSlot>();
            
            if (slotScript != null)
            {
                slotScript.Setup(item);
                slotScript.SetUnlocked(false); // Na start wszystko wyszarzone
                uiSlots.Add(slotScript);
            }
        }
    }

    // Metoda wywoływana, gdy gracz podniesie znajdźkę
    public void UnlockCollectible(string itemID)
    {
        if (!unlockedCollectibleIDs.Contains(itemID))
        {
            unlockedCollectibleIDs.Add(itemID);
            RefreshJournalUI();
            Debug.Log($"[Journal] Odblokowano nową znajdźkę: {itemID}");
        }
    }

    // Aktualizuje wygląd wszystkich slotów, np. po otwarciu dziennika
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

    // Funkcja podpinana pod przycisk "otwórz dziennik" z Twojego mockupu czasu
    public void ToggleJournal()
    {
        bool isActive = !journalPanel.activeSelf;
        journalPanel.SetActive(isActive);

        if (isActive)
        {
            RefreshJournalUI();
        }
    }
    public void CloseJournal()
    {
        journalPanel.SetActive(false);
    }
}
