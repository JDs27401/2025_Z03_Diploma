using Player.scripts;
using UnityEngine;

namespace C__Classes.SaveSystem
{
    public static class SaveStateMapper
    {
        public static WeaponInstanceSaveData CaptureWeaponState(WeaponInstanceState state)
        {
            if (state == null)
            {
                return null;
            }

            WeaponInstanceSaveData saveData = new WeaponInstanceSaveData
            {
                currentMagazineAmmo = state.currentMagazineAmmo,
                runtimeStats = state.runtimeStats != null ? state.runtimeStats.Clone() : null
            };

            if (state.installedMods != null)
            {
                for (int i = 0; i < state.installedMods.Count; i++)
                {
                    WeaponModInstanceState modState = state.installedMods[i];
                    if (modState != null && modState.itemData != null && !string.IsNullOrWhiteSpace(modState.itemData.id))
                    {
                        saveData.installedModItemIds.Add(modState.itemData.id);
                    }
                }
            }

            return saveData;
        }

        public static MeleeWeaponInstanceSaveData CaptureMeleeWeaponState(MeleeWeaponInstanceState state)
        {
            if (state == null)
            {
                return null;
            }

            MeleeWeaponInstanceSaveData saveData = new MeleeWeaponInstanceSaveData
            {
                runtimeStats = state.runtimeStats != null ? state.runtimeStats.Clone() : null
            };

            if (state.installedMods != null)
            {
                for (int i = 0; i < state.installedMods.Count; i++)
                {
                    MeleeWeaponModInstanceState modState = state.installedMods[i];
                    if (modState != null && modState.itemData != null && !string.IsNullOrWhiteSpace(modState.itemData.id))
                    {
                        saveData.installedModItemIds.Add(modState.itemData.id);
                    }
                }
            }

            return saveData;
        }

        public static WeaponInstanceState RestoreWeaponState(WeaponInstanceSaveData saveData, ItemData item, ItemDatabase itemDatabase)
        {
            if (saveData == null || !(item is WeaponItemData weaponItemData) || weaponItemData.weaponData == null)
            {
                return null;
            }

            WeaponInstanceState state = new WeaponInstanceState(saveData.currentMagazineAmmo);
            state.InitializeFromWeaponData(weaponItemData.weaponData);
            state.installedMods.Clear();

            if (saveData.installedModItemIds != null)
            {
                for (int i = 0; i < saveData.installedModItemIds.Count; i++)
                {
                    WeaponModItemData modItem = itemDatabase.GetItemById<WeaponModItemData>(saveData.installedModItemIds[i]);
                    if (modItem != null)
                    {
                        state.installedMods.Add(new WeaponModInstanceState(modItem));
                    }
                }
            }

            state.RebuildRuntimeStats();
            if (saveData.runtimeStats != null)
            {
                state.runtimeStats = saveData.runtimeStats.Clone();
            }

            state.currentMagazineAmmo = Mathf.Clamp(saveData.currentMagazineAmmo, 0, state.GetRuntimeStats().magazineSize);
            return state;
        }

        public static MeleeWeaponInstanceState RestoreMeleeWeaponState(MeleeWeaponInstanceSaveData saveData, ItemData item, ItemDatabase itemDatabase)
        {
            if (saveData == null || !(item is MeleeWeaponItemData meleeWeaponItemData) || meleeWeaponItemData.meleeWeaponData == null)
            {
                return null;
            }

            MeleeWeaponInstanceState state = new MeleeWeaponInstanceState();
            state.InitializeFromMeleeData(meleeWeaponItemData.meleeWeaponData);
            state.installedMods.Clear();

            if (saveData.installedModItemIds != null)
            {
                for (int i = 0; i < saveData.installedModItemIds.Count; i++)
                {
                    MeleeWeaponModItemData modItem = itemDatabase.GetItemById<MeleeWeaponModItemData>(saveData.installedModItemIds[i]);
                    if (modItem != null)
                    {
                        state.installedMods.Add(new MeleeWeaponModInstanceState(modItem));
                    }
                }
            }

            state.RebuildRuntimeStats();
            if (saveData.runtimeStats != null)
            {
                state.runtimeStats = saveData.runtimeStats.Clone();
            }

            return state;
        }
    }
}
