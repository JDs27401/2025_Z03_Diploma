using C__Classes.Systems;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Wymagane dla świateł URP 2D

public class LightningSystem : MonoBehaviour
{
    [SerializeField] private Light2D globalLight2D;
    
    [SerializeField] private Gradient dailyColor;

    private float minutesInDay = 1440f;
    void Update()
    {
        float hours = Universe.GetHour();
        float minutes = Universe.GetMinute();
        float dayProgressionInMinutes = hours * 60 + minutes;
        float timePercentOfDayProgression =  dayProgressionInMinutes/minutesInDay;

        if (globalLight2D != null)
        {
            globalLight2D.color = dailyColor.Evaluate(timePercentOfDayProgression);
        }
    }
}