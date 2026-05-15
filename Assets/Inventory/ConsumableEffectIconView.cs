using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace C__Classes.Managers
{
    public class ConsumableEffectIconView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI timerText;

        private void Awake()
        {
            if (iconImage == null)
            {
                iconImage = GetComponent<Image>();
            }

            EnsureTimerText();
        }

        public void Bind(ConsumableEffectInstance effectInstance)
        {
            if (effectInstance == null || effectInstance.effectData == null)
            {
                return;
            }

            if (iconImage != null)
            {
                iconImage.sprite = effectInstance.Icon;
                iconImage.enabled = effectInstance.Icon != null;
            }

            EnsureTimerText();
            UpdateTimer(effectInstance);
        }

        public void UpdateTimer(ConsumableEffectInstance effectInstance)
        {
            if (timerText == null || effectInstance == null)
            {
                return;
            }

            timerText.text = Mathf.CeilToInt(Mathf.Max(0f, effectInstance.remainingTime)).ToString();
        }

        private void EnsureTimerText()
        {
            if (timerText != null)
            {
                return;
            }

            GameObject timerGo = new GameObject("Timer", typeof(RectTransform));
            timerGo.transform.SetParent(transform, false);

            timerText = timerGo.AddComponent<TextMeshProUGUI>();
            timerText.alignment = TextAlignmentOptions.BottomRight;
            timerText.fontSize = 16f;
            timerText.color = Color.white;
            timerText.raycastTarget = false;

            RectTransform rect = timerText.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}


