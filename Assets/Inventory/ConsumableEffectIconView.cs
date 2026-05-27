using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace C__Classes.Managers
{
    public class ConsumableEffectIconView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI timerText;
        [Header("Optional font override")]
        [SerializeField] private TMP_FontAsset overrideFont;

        // Allow external code to set override font (e.g. from HUD)
        public TMP_FontAsset OverrideFont
        {
            set
            {
                overrideFont = value;
                if (timerText != null && overrideFont != null)
                {
                    timerText.font = overrideFont;
                }
            }
        }
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
            // timerText.fontMaterial = new Material(timerText.fontSharedMaterial);
            // timerText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 5f);
            // timerText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
            timerText.fontSize = 16f;
            timerText.color = Color.white;
            timerText.raycastTarget = false;

            // Try to assign override font if provided, otherwise attempt to find font by name
            if (overrideFont != null)
            {
                timerText.font = overrideFont;
            }
            else
            {
                // Attempt to find font asset named exactly "04B_03__ SDF" in the project (editor/runtime fallback)
                var allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                for (int i = 0; i < allFonts.Length; i++)
                {
                    if (allFonts[i] != null && allFonts[i].name == "04B_03__ SDF")
                    {
                        timerText.font = allFonts[i];
                        break;
                    }
                }
            }
            RectTransform rect = timerText.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}


