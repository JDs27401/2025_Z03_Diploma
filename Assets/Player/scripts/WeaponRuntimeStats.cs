namespace Player.scripts
{
    using UnityEngine;

    [System.Serializable]
    public class WeaponRuntimeStats
    {
        public string weaponName = "New Weapon";
        public int animationID;

        public float fireRate = 4f;
        public float damage = 10f;
        public float projectileSpeed = 15f;
        public float spread = 0f;
        public int projectilesPerShot = 1;

        public bool isAutomatic = false;

        public int magazineSize = 10;
        public float reloadTime = 2f;

        public bool isExplosive = false;
        public float explosionRadius = 3f;

        public bool isMolotov = false;
        public float dotAreaRadius = 2.5f;
        public float dotDamage = 4f;
        public float dotDuration = 4f;
        public float dotInterval = 1f;
        public float dotAreaLifetime = 5f;
        
        // Final runtime weight of the weapon (base weight from WeaponData modified by mods)
        public float weight = 1.0f;

        public WeaponRuntimeStats Clone()
        {
            return new WeaponRuntimeStats
            {
                weaponName = weaponName,
                animationID = animationID,
                fireRate = fireRate,
                damage = damage,
                projectileSpeed = projectileSpeed,
                spread = spread,
                projectilesPerShot = projectilesPerShot,
                isAutomatic = isAutomatic,
                magazineSize = magazineSize,
                reloadTime = reloadTime,
                isExplosive = isExplosive,
                explosionRadius = explosionRadius,
                isMolotov = isMolotov,
                dotAreaRadius = dotAreaRadius,
                dotDamage = dotDamage,
                dotDuration = dotDuration,
                dotInterval = dotInterval,
                dotAreaLifetime = dotAreaLifetime,
                weight = weight
            };
        }

        public void CopyFrom(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                return;
            }

            weaponName = weaponData.weaponName;
            animationID = weaponData.animationID;
            fireRate = weaponData.fireRate;
            damage = weaponData.damage;
            projectileSpeed = weaponData.projectileSpeed;
            spread = weaponData.spread;
            projectilesPerShot = weaponData.projectilesPerShot;
            isAutomatic = weaponData.isAutomatic;
            magazineSize = weaponData.magazineSize;
            reloadTime = weaponData.reloadTime;
            isExplosive = weaponData.isExplosive;
            explosionRadius = weaponData.explosionRadius;
            isMolotov = weaponData.isMolotov;
            dotAreaRadius = weaponData.dotAreaRadius;
            dotDamage = weaponData.dotDamage;
            dotDuration = weaponData.dotDuration;
            dotInterval = weaponData.dotInterval;
            dotAreaLifetime = weaponData.dotAreaLifetime;
            weight = weaponData.weight;
            Normalize();
        }

        public void ApplyMod(WeaponModData modData)
        {
            if (modData == null)
            {
                return;
            }

            fireRate += modData.fireRateBonus;
            damage += modData.damageBonus;
            projectileSpeed += modData.projectileSpeedBonus;
            spread += modData.spreadBonus;
            projectilesPerShot += modData.projectilesPerShotBonus;
            magazineSize += modData.magazineSizeBonus;
            reloadTime += modData.reloadTimeBonus;
            explosionRadius += modData.explosionRadiusBonus;
            dotAreaRadius += modData.dotAreaRadiusBonus;
            dotDamage += modData.dotDamageBonus;
            dotDuration += modData.dotDurationBonus;
            dotInterval += modData.dotIntervalBonus;
            dotAreaLifetime += modData.dotAreaLifetimeBonus;
            // weight percent modifiers are applied once in WeaponInstanceState.RebuildRuntimeStats()

            if (modData.overrideIsAutomatic)
            {
                isAutomatic = modData.isAutomaticValue;
            }

            if (modData.overrideIsExplosive)
            {
                isExplosive = modData.isExplosiveValue;
            }

            if (modData.overrideIsMolotov)
            {
                isMolotov = modData.isMolotovValue;
            }

            Normalize();
        }

        public void Normalize()
        {
            fireRate = Mathf.Max(0.1f, fireRate);
            damage = Mathf.Max(0f, damage);
            projectileSpeed = Mathf.Max(0f, projectileSpeed);
            spread = Mathf.Max(0f, spread);
            projectilesPerShot = Mathf.Max(1, projectilesPerShot);
            magazineSize = Mathf.Max(1, magazineSize);
            reloadTime = Mathf.Max(0.05f, reloadTime);
            explosionRadius = Mathf.Max(0f, explosionRadius);
            dotAreaRadius = Mathf.Max(0f, dotAreaRadius);
            dotDamage = Mathf.Max(0f, dotDamage);
            dotDuration = Mathf.Max(0f, dotDuration);
            dotInterval = Mathf.Max(0.01f, dotInterval);
            dotAreaLifetime = Mathf.Max(0f, dotAreaLifetime);
            weight = Mathf.Max(0f, weight);
        }
    }
}

