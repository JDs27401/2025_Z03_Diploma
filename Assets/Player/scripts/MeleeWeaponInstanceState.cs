namespace Player.scripts
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class MeleeWeaponInstanceState
    {
        public MeleeWeaponData sourceMeleeData;
        public MeleeWeaponRuntimeStats runtimeStats = new MeleeWeaponRuntimeStats();
        public List<MeleeWeaponModInstanceState> installedMods = new List<MeleeWeaponModInstanceState>();

        public const int MaxInstalledMods = 3;

        public MeleeWeaponInstanceState() { }

        public void InitializeFromMeleeData(MeleeWeaponData data)
        {
            sourceMeleeData = data;
            if (runtimeStats == null) runtimeStats = new MeleeWeaponRuntimeStats();
            runtimeStats.CopyFrom(data);
            RebuildRuntimeStats();
        }

        public bool CanInstallMoreMods()
        {
            return installedMods != null && installedMods.Count < MaxInstalledMods;
        }

        public MeleeWeaponModInstanceState InstallMod(MeleeWeaponModItemData modItemData)
        {
            if (modItemData == null || modItemData.meleeWeaponModData == null || !CanInstallMoreMods())
            {
                return null;
            }

            MeleeWeaponModInstanceState modState = new MeleeWeaponModInstanceState(modItemData);
            installedMods.Add(modState);
            RebuildRuntimeStats();
            return modState;
        }

        public bool RemoveMod(MeleeWeaponModInstanceState modState)
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

        public bool RestoreMod(MeleeWeaponModInstanceState modState)
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

        public MeleeWeaponModInstanceState RemoveModAt(int index)
        {
            if (installedMods == null || index < 0 || index >= installedMods.Count)
            {
                return null;
            }

            MeleeWeaponModInstanceState modState = installedMods[index];
            installedMods.RemoveAt(index);
            RebuildRuntimeStats();
            return modState;
        }

        public void RebuildRuntimeStats()
        {
            if (runtimeStats == null)
            {
                runtimeStats = new MeleeWeaponRuntimeStats();
            }

            runtimeStats.CopyFrom(sourceMeleeData);

            if (installedMods == null)
            {
                return;
            }

            float totalWeightPercent = 0f;
            for (int i = 0; i < installedMods.Count; i++)
            {
                MeleeWeaponModInstanceState modState = installedMods[i];
                if (modState != null && modState.modData != null)
                {
                    runtimeStats.ApplyMod(modState.modData);
                    totalWeightPercent += modState.modData.weightPercentBonus;
                }
            }

            runtimeStats.weight = Mathf.Max(0f, runtimeStats.weight * (1f + totalWeightPercent));
            runtimeStats.Normalize();
        }

        public MeleeWeaponRuntimeStats GetRuntimeStats()
        {
            if (runtimeStats == null)
            {
                runtimeStats = new MeleeWeaponRuntimeStats();
                runtimeStats.CopyFrom(sourceMeleeData);
            }

            return runtimeStats;
        }

        public MeleeWeaponInstanceState Clone()
        {
            MeleeWeaponInstanceState clone = new MeleeWeaponInstanceState
            {
                sourceMeleeData = sourceMeleeData,
                runtimeStats = runtimeStats != null ? runtimeStats.Clone() : null,
                installedMods = new List<MeleeWeaponModInstanceState>()
            };

            if (installedMods != null)
            {
                for (int i = 0; i < installedMods.Count; i++)
                {
                    MeleeWeaponModInstanceState modState = installedMods[i];
                    if (modState != null)
                    {
                        clone.installedMods.Add(modState.Clone());
                    }
                }
            }

            return clone;
        }

        public MeleeWeaponModInstanceState GetModByType(MeleeWeaponModType modType)
        {
            if (installedMods == null)
            {
                return null;
            }

            for (int i = 0; i < installedMods.Count; i++)
            {
                MeleeWeaponModInstanceState modState = installedMods[i];
                if (modState != null && modState.modData != null && modState.modData.modType == modType)
                {
                    return modState;
                }
            }

            return null;
        }

        public List<MeleeWeaponModInstanceState> GetModsByType(MeleeWeaponModType modType)
        {
            List<MeleeWeaponModInstanceState> result = new List<MeleeWeaponModInstanceState>();

            if (installedMods == null)
            {
                return result;
            }

            for (int i = 0; i < installedMods.Count; i++)
            {
                MeleeWeaponModInstanceState modState = installedMods[i];
                if (modState != null && modState.modData != null && modState.modData.modType == modType)
                {
                    result.Add(modState);
                }
            }

            return result;
        }

        public bool HasModType(MeleeWeaponModType modType)
        {
            return GetModByType(modType) != null;
        }
    }
}

