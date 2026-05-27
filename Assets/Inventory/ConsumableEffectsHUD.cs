using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace C__Classes.Managers
{
    public class ConsumableEffectsHUD : MonoBehaviour
    {
        [SerializeField] private Vector2 iconSize = new Vector2(44f, 44f);
        [SerializeField] private float spacing = 6f;
        [Header("Font")]
        [SerializeField] private TMPro.TMP_FontAsset _fontAsset;

        public TMPro.TMP_FontAsset FontAsset
        {
            set { _fontAsset = value; }
        }

        private readonly Dictionary<string, ConsumableEffectIconView> _effectViews = new Dictionary<string, ConsumableEffectIconView>();
        private RectTransform _rootRect;
        private ConsumableEffectManager _effectManager;
        private HorizontalLayoutGroup _layoutGroup;

        private void Awake()
        {
            _rootRect = GetComponent<RectTransform>();
            if (_rootRect == null)
            {
                _rootRect = gameObject.AddComponent<RectTransform>();
            }

            _layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (_layoutGroup == null)
            {
                _layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            _layoutGroup.spacing = spacing;
            _layoutGroup.childAlignment = TextAnchor.LowerLeft;
            _layoutGroup.childForceExpandWidth = false;
            _layoutGroup.childForceExpandHeight = false;
            _layoutGroup.childControlWidth = false;
            _layoutGroup.childControlHeight = false;
            _layoutGroup.childScaleWidth = false;
            _layoutGroup.childScaleHeight = false;
        }

        public void Initialize(ConsumableEffectManager effectManager)
        {
            _effectManager = effectManager;
            EnsureAnchors();
        }

        private void EnsureAnchors()
        {
            if (_rootRect == null)
            {
                _rootRect = GetComponent<RectTransform>();
            }

            if (_rootRect == null)
            {
                return;
            }

            _rootRect.anchorMin = new Vector2(0f, 0f);
            _rootRect.anchorMax = new Vector2(0f, 0f);
            _rootRect.pivot = new Vector2(0f, 0f);
            _rootRect.anchoredPosition = new Vector2(16f, 16f);
        }

        private void Update()
        {
            if (_effectManager == null)
            {
                return;
            }

            Refresh(_effectManager.ActiveEffects);
        }

        public void Refresh(IReadOnlyList<ConsumableEffectInstance> activeEffects)
        {
            HashSet<string> activeIds = new HashSet<string>();

            for (int i = 0; i < activeEffects.Count; i++)
            {
                ConsumableEffectInstance effect = activeEffects[i];
                if (effect == null || effect.effectData == null || string.IsNullOrWhiteSpace(effect.EffectId))
                {
                    continue;
                }

                activeIds.Add(effect.EffectId);
                ConsumableEffectIconView view = GetOrCreateView(effect.EffectId, i);
                view.Bind(effect);
                view.UpdateTimer(effect);
                view.transform.SetSiblingIndex(i);
            }

            List<string> existingIds = _effectViews.Keys.ToList();
            for (int i = 0; i < existingIds.Count; i++)
            {
                string id = existingIds[i];
                if (activeIds.Contains(id))
                {
                    continue;
                }

                if (_effectViews.TryGetValue(id, out ConsumableEffectIconView view) && view != null)
                {
                    Destroy(view.gameObject);
                }

                _effectViews.Remove(id);
            }
        }

        private ConsumableEffectIconView GetOrCreateView(string effectId, int siblingIndex)
        {
            if (_effectViews.TryGetValue(effectId, out ConsumableEffectIconView existing) && existing != null)
            {
                return existing;
            }

            GameObject iconGo = new GameObject($"Effect_{effectId}");
            iconGo.transform.SetParent(transform, false);

            RectTransform rect = iconGo.AddComponent<RectTransform>();
            rect.sizeDelta = iconSize;
            rect.localScale = Vector3.one;

            Image image = iconGo.AddComponent<Image>();
            image.raycastTarget = false;

            ConsumableEffectIconView view = iconGo.AddComponent<ConsumableEffectIconView>();
            // If HUD has a font asset configured, apply it to the created view
            if (_fontAsset != null)
            {
                view.OverrideFont = _fontAsset;
            }
            iconGo.transform.SetSiblingIndex(siblingIndex);

            _effectViews[effectId] = view;
            return view;
        }
    }
}


