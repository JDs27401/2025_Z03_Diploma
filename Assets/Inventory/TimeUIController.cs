using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Button journalButton;

    [Header("Settings")]
    [SerializeField] private string timePrefix = "Time ";

    void Start()
    {
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
        int hours = C__Classes.Systems.Universe.GetHour();
        int minutes = (int)C__Classes.Systems.Universe.GetMinute();

        // Time formatting 00:00
        timeText.text = $"{timePrefix}{hours:D2}:{minutes:D2}";
        
        if (dayText != null)
        {
            dayText.text = C__Classes.Systems.Universe.GetDay().ToString();
        }
    }

    private void OpenJournal()
    {
        JournalManager.Instance.ToggleJournal();
    }
}