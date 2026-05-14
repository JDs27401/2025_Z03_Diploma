namespace Player.scripts
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class WeaponInstanceState
    {
        public int currentMagazineAmmo;
        public WeaponData sourceWeaponData;
        public WeaponRuntimeStats runtimeStats = new WeaponRuntimeStats();
        public List<WeaponModInstanceState> installedMods = new List<WeaponModInstanceState>();

        public const int MaxInstalledMods = 3;

        public WeaponInstanceState(int currentMagazineAmmo)
        {
            this.currentMagazineAmmo = currentMagazineAmmo;
        }

        public void InitializeFromWeaponData(WeaponData weaponData)
        {
            sourceWeaponData = weaponData;

            if (runtimeStats == null)
            {
                runtimeStats = new WeaponRuntimeStats();
            }

            runtimeStats.CopyFrom(weaponData);
            RebuildRuntimeStats();

            if (weaponData != null)
            {
                currentMagazineAmmo = UnityEngine.Mathf.Clamp(currentMagazineAmmo, 0, runtimeStats.magazineSize);
            }
        }

        public bool CanInstallMoreMods()
        {
            return installedMods != null && installedMods.Count < MaxInstalledMods;
        }

        public WeaponModInstanceState InstallMod(WeaponModItemData modItemData)
        {
            if (modItemData == null || modItemData.weaponModData == null || !CanInstallMoreMods())
            {
                return null;
            }

            WeaponModInstanceState modState = new WeaponModInstanceState(modItemData);
            installedMods.Add(modState);
            RebuildRuntimeStats();
            return modState;
        }

        public bool RemoveMod(WeaponModInstanceState modState)
        {
            if (modState == null || installedMods == null)
            {
                return false;
            }

            bool removed = installedMods.Remove(modState);
            if (removed)
            {
                RebuildRuntimeStats();
            }

            return removed;
        }

        public bool RestoreMod(WeaponModInstanceState modState)
        {
            if (modState == null || !CanInstallMoreMods())
            {
                return false;
            }

            if (installedMods.Contains(modState))
            {
                return true;
            }

            installedMods.Add(modState);
            RebuildRuntimeStats();
            return true;
        }

        public WeaponModInstanceState RemoveModAt(int index)
        {
            if (installedMods == null || index < 0 || index >= installedMods.Count)
            {
                return null;
            }

            WeaponModInstanceState modState = installedMods[index];
            installedMods.RemoveAt(index);
            RebuildRuntimeStats();
            return modState;
        }

        public void RebuildRuntimeStats()
        {
            if (runtimeStats == null)
            {
                runtimeStats = new WeaponRuntimeStats();
            }

            runtimeStats.CopyFrom(sourceWeaponData);

            if (installedMods == null)
            {
                return;
            }
            // Apply all stat bonuses from mods (non-weight) and accumulate weight percent bonuses
            float totalWeightPercent = 0f;
            for (int i = 0; i < installedMods.Count; i++)
            {
                WeaponModInstanceState modState = installedMods[i];
                if (modState != null && modState.modData != null)
                {
                    runtimeStats.ApplyMod(modState.modData);
                    totalWeightPercent += modState.modData.weightPercentBonus;
                }
            }

            // Apply percentual weight modification once (sum of percentages). Ensure weight >= 0
            runtimeStats.weight = Mathf.Max(0f, runtimeStats.weight * (1f + totalWeightPercent));

            runtimeStats.Normalize();
            currentMagazineAmmo = UnityEngine.Mathf.Clamp(currentMagazineAmmo, 0, runtimeStats.magazineSize);
        }

        public WeaponRuntimeStats GetRuntimeStats()
        {
            if (runtimeStats == null)
            {
                runtimeStats = new WeaponRuntimeStats();
                runtimeStats.CopyFrom(sourceWeaponData);
            }

            return runtimeStats;
        }

        public WeaponInstanceState Clone()
        {
            WeaponInstanceState clone = new WeaponInstanceState(currentMagazineAmmo)
            {
                sourceWeaponData = sourceWeaponData,
                runtimeStats = runtimeStats != null ? runtimeStats.Clone() : null,
                installedMods = new List<WeaponModInstanceState>()
            };

            if (installedMods != null)
            {
                for (int i = 0; i < installedMods.Count; i++)
                {
                    WeaponModInstanceState modState = installedMods[i];
                    if (modState != null)
                    {
                        clone.installedMods.Add(modState.Clone());
                    }
                }
            }

            return clone;
        }

        public WeaponModInstanceState GetModByType(WeaponModType modType)
        {
            if (installedMods == null)
            {
                return null;
            }

            for (int i = 0; i < installedMods.Count; i++)
            {
                WeaponModInstanceState modState = installedMods[i];
                if (modState != null && modState.modData != null && modState.modData.modType == modType)
                {
                    return modState;
                }
            }

            return null;
        }

        public List<WeaponModInstanceState> GetModsByType(WeaponModType modType)
        {
            List<WeaponModInstanceState> result = new List<WeaponModInstanceState>();

            if (installedMods == null)
            {
                return result;
            }

            for (int i = 0; i < installedMods.Count; i++)
            {
                WeaponModInstanceState modState = installedMods[i];
                if (modState != null && modState.modData != null && modState.modData.modType == modType)
                {
                    result.Add(modState);
                }
            }

            return result;
        }

        public bool HasModType(WeaponModType modType)
        {
            return GetModByType(modType) != null;
        }
    }
}

