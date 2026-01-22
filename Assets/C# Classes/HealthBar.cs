using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public Gradient gradient;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    public void UpdateHealthBar(float healthPercentage)
    {
        slider.value = healthPercentage;

        if (fillImage != null && gradient != null)
        {
            fillImage.color = gradient.Evaluate(healthPercentage);
        }
    }
}