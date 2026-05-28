namespace Player.scripts
{
    using UnityEngine;

    [System.Serializable]
    public class MeleeWeaponInstanceState
    {
        public MeleeWeaponData sourceMeleeData;
        public MeleeWeaponRuntimeStats runtimeStats = new MeleeWeaponRuntimeStats();

        public MeleeWeaponInstanceState() { }

        public void InitializeFromMeleeData(MeleeWeaponData data)
        {
            sourceMeleeData = data;
            if (runtimeStats == null) runtimeStats = new MeleeWeaponRuntimeStats();
            runtimeStats.CopyFrom(data);
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
                runtimeStats = runtimeStats != null ? runtimeStats.Clone() : null
            };

            return clone;
        }
    }
}

