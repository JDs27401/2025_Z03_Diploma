using Unity.VisualScripting.FullSerializer;

namespace Player.scripts
{
    using UnityEngine;

    [System.Serializable]
    public class MeleeWeaponRuntimeStats
    {
        public string weaponName = "New Melee Weapon";
        public int animationID;

        public float attackRate = 1.5f;
        public float damage = 25f;
        public float range = 1.0f;
        public float angle = 120f;
        public float hitboxDuration = 0.2f;
        public int staminaCost = 5;

        public bool isAutomatic = false;

        public float weight = 1.0f;

        public bool isMolotov = false;
        public float dotAreaRadius = 2.5f;
        public float dotDamage = 4f;
        public float dotDuration = 4f;
        public float dotInterval = 1f;
        public float dotAreaLifetime = 5f;

        public MeleeWeaponRuntimeStats Clone()
        {
            return new MeleeWeaponRuntimeStats
            {
                weaponName = weaponName,
                animationID = animationID,
                attackRate = attackRate,
                damage = damage,
                range = range,
                angle = angle,
                hitboxDuration = hitboxDuration,
                staminaCost = staminaCost,
                isAutomatic = isAutomatic,
                weight = weight
                ,isMolotov = isMolotov
                ,dotAreaRadius = dotAreaRadius
                ,dotDamage = dotDamage
                ,dotDuration = dotDuration
                ,dotInterval = dotInterval
                ,dotAreaLifetime = dotAreaLifetime
            };
        }

        public void CopyFrom(MeleeWeaponData data)
        {
            if (data == null) return;

            weaponName = data.weaponName;
            animationID = data.animationID;
            attackRate = data.attackRate;
            damage = data.damage;
            range = data.range;
            angle = data.angle;
            hitboxDuration = data.hitboxDuration;
            staminaCost = data.staminaCost;
            isAutomatic = false; // default; melee-specific mods may override later
            weight = data.weight;
            isMolotov = data.isMolotov;
            dotAreaRadius = data.dotAreaRadius;
            dotDamage = data.dotDamage;
            dotDuration = data.dotDuration;
            dotInterval = data.dotInterval;
            dotAreaLifetime = data.dotAreaLifetime;
            Normalize();
        }

        public void ApplyMod(MeleeWeaponModData modData)
        {
            if (modData == null)
            {
                return;
            }

            attackRate += modData.attackRateBonus;
            damage += modData.damageBonus;
            range += modData.rangeBonus;
            angle += modData.angleBonus;
            hitboxDuration += modData.hitboxDurationBonus;
            staminaCost += modData.staminaCostBonus;
            // weight percent modifiers are applied once in MeleeWeaponInstanceState.RebuildRuntimeStats()

            if (modData.overrideIsAutomatic)
            {
                isAutomatic = modData.isAutomaticValue;
            }
            if (modData.overrideIsMolotov)
            {
                isMolotov = modData.isMolotovValue;
            }

            dotAreaRadius += modData.dotAreaRadiusBonus;
            dotDamage += modData.dotDamageBonus;
            dotDuration += modData.dotDurationBonus;
            dotInterval += modData.dotIntervalBonus;
            dotAreaLifetime += modData.dotAreaLifetimeBonus;

            Normalize();
        }

        public void Normalize()
        {
            attackRate = Mathf.Max(0.01f, attackRate);
            damage = Mathf.Max(0f, damage);
            range = Mathf.Max(0f, range);
            angle = Mathf.Clamp(angle, 0f, 360f);
            hitboxDuration = Mathf.Max(0f, hitboxDuration);
            staminaCost = Mathf.Max(0, staminaCost);
            weight = Mathf.Max(0f, weight);
            dotAreaRadius = Mathf.Max(0f, dotAreaRadius);
            dotDamage = Mathf.Max(0f, dotDamage);
            dotDuration = Mathf.Max(0f, dotDuration);
            dotInterval = Mathf.Max(0.01f, dotInterval);
            dotAreaLifetime = Mathf.Max(0f, dotAreaLifetime);
        }
    }
}

