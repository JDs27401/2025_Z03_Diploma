using UnityEngine;
using TMPro; // Upewnij się, że używasz TextMeshPro
using UnityEngine.UI;

public class TimeUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText; // Opcjonalnie, jeśli chcesz wyświetlać numer dnia pod kalendarzem
    [SerializeField] private Button journalButton;

    [Header("Settings")]
    [SerializeField] private string timePrefix = "Time ";

    void Start()
    {
        // Przypisanie funkcji do przycisku dziennika
        if (journalButton != null)
        {
            journalButton.onClick.AddListener(OpenJournal);
        }
    }

    void Update()
    {
        UpdateDateTimeDisplay();
    }

    private void UpdateDateTimeDisplay()
    {
        // Pobieranie danych z Twojej klasy Universe
        int hours = C__Classes.Systems.Universe.GetHour();
        int minutes = (int)C__Classes.Systems.Universe.GetMinute();

        // Formatowanie czasu na 00:00
        timeText.text = $"{timePrefix}{hours:D2}:{minutes:D2}";
        
        // Jeśli masz miejsce na numer dnia (np. na ikonie kalendarza)
        if (dayText != null)
        {
            dayText.text = C__Classes.Systems.Universe.GetDay().ToString();
        }
    }

    private void OpenJournal()
    {
        Debug.Log("Otwieranie dziennika...");
        JournalManager.Instance.ToggleJournal();
    }
}