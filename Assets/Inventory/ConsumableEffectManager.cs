using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace C__Classes.Managers
{
    [System.Serializable]
    public class ConsumableEffectInstance
    {
        public ConsumableEffectData effectData;
        public float remainingTime;

        public ConsumableEffectInstance(ConsumableEffectData effectData)
        {
            this.effectData = effectData;
            remainingTime = effectData != null ? effectData.duration : 0f;
        }

        public string EffectId
        {
            get
            {
                if (effectData == null)
                {
                    return string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(effectData.effectId))
                {
                    return effectData.effectId;
                }

                return effectData.name;
            }
        }

        public Sprite Icon => effectData != null ? effectData.icon : null;

        public float Duration => effectData != null ? effectData.duration : 0f;

        public bool Tick(float deltaTime)
        {
            remainingTime -= deltaTime;
            return remainingTime <= 0f;
        }

        public void ExtendByFullDuration()
        {
            if (effectData != null)
            {
                remainingTime += effectData.duration;
            }
        }
    }

    public class ConsumableEffectManager
    {
        // Event fired when active effects change (applied / extended / expired)
        public event Action OnEffectsChanged;

        private readonly List<ConsumableEffectInstance> _activeEffects = new List<ConsumableEffectInstance>();
        private readonly Dictionary<string, ConsumableEffectInstance> _effectLookup = new Dictionary<string, ConsumableEffectInstance>();

        public IReadOnlyList<ConsumableEffectInstance> ActiveEffects => _activeEffects;

        public bool ApplyConsumable(ConsumableItemData itemData)
        {
            if (itemData == null || itemData.effects == null || itemData.effects.Count == 0)
            {
                return false;
            }

            bool appliedAny = false;
            for (int i = 0; i < itemData.effects.Count; i++)
            {
                appliedAny |= ApplyEffect(itemData.effects[i]);
            }

            return appliedAny;
        }

        public bool ApplyEffect(ConsumableEffectData effectData)
        {
            if (effectData == null)
            {
                return false;
            }

            string effectId = GetEffectId(effectData);
            if (string.IsNullOrWhiteSpace(effectId))
            {
                return false;
            }

            if (_effectLookup.TryGetValue(effectId, out ConsumableEffectInstance existing))
            {
                existing.ExtendByFullDuration();
                OnEffectsChanged?.Invoke();
                return true;
            }

            ConsumableEffectInstance instance = new ConsumableEffectInstance(effectData);
            _activeEffects.Add(instance);
            _effectLookup[effectId] = instance;
            OnEffectsChanged?.Invoke();
            return true;
        }

        private string GetEffectId(ConsumableEffectData effectData)
        {
            if (effectData == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(effectData.effectId))
            {
                return effectData.effectId;
            }

            return effectData.name;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || _activeEffects.Count == 0)
            {
                return;
            }

            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance == null)
                {
                    _activeEffects.RemoveAt(i);
                    continue;
                }

                if (instance.Tick(deltaTime))
                {
                    if (!string.IsNullOrWhiteSpace(instance.EffectId))
                    {
                        _effectLookup.Remove(instance.EffectId);
                    }

                    _activeEffects.RemoveAt(i);
                    OnEffectsChanged?.Invoke();
                }
            }
        }

        public void Clear()
        {
            _activeEffects.Clear();
            _effectLookup.Clear();
        }

        public float GetSpeedMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null)
                {
                    continue;
                }

                multiplier *= Mathf.Max(0f, instance.effectData.speedMultiplier);
            }

            return multiplier;
        }

        public float GetMaxStaminaBonus()
        {
            float bonus = 0f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null) continue;
                bonus += instance.effectData.maxStaminaBonus;
            }

            return bonus;
        }

        public float GetStaminaPerSecond()
        {
            float regen = 0f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null) continue;
                regen += instance.effectData.staminaPerSecond;
            }

            return regen;
        }

        public float GetDamageTakenMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null) continue;
                multiplier *= Mathf.Max(0f, instance.effectData.damageTakenMultiplier);
            }

            return multiplier;
        }

        public float GetNoiseMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null) continue;
                multiplier *= Mathf.Max(0f, instance.effectData.noiseMultiplier);
            }

            return multiplier;
        }

        public float GetWeaponSpreadMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null) continue;
                multiplier *= Mathf.Max(0f, instance.effectData.weaponSpreadMultiplier);
            }

            return multiplier;
        }

        public float GetAccelerationMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null)
                {
                    continue;
                }

                multiplier *= Mathf.Max(0f, instance.effectData.accelerationMultiplier);
            }

            return multiplier;
        }

        public float GetMaxHealthBonus()
        {
            float bonus = 0f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null)
                {
                    continue;
                }

                bonus += instance.effectData.maxHealthBonus;
            }

            return bonus;
        }

        public float GetHealthPerSecond()
        {
            float regen = 0f;
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ConsumableEffectInstance instance = _activeEffects[i];
                if (instance?.effectData == null)
                {
                    continue;
                }

                regen += instance.effectData.healthPerSecond;
            }

            return regen;
        }

        public List<ConsumableEffectInstance> GetActiveEffectsSnapshot()
        {
            return _activeEffects.Where(effect => effect != null).ToList();
        }
    }
}



