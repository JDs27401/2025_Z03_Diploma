namespace Player.scripts
{
    using UnityEngine;

    public enum MeleeWeaponModType
    {
        Standard = 0,
        Lightweight = 1,
        Heavy = 2,
        Bleed = 3
    }

    [CreateAssetMenu(fileName = "New Melee Weapon Mod", menuName = "Weapons/Melee Weapon Mod")]
    public class MeleeWeaponModData : ScriptableObject
    {
        public string modName = "New Melee Weapon Mod";
        public Sprite icon;
        public MeleeWeaponModType modType = MeleeWeaponModType.Standard;

        [TextArea]
        public string description;

        [Header("Melee Stats")]
        public float attackRateBonus = 0f;
        public float damageBonus = 0f;
        public float rangeBonus = 0f;
        public float angleBonus = 0f;
        public float hitboxDurationBonus = 0f;
        public int staminaCostBonus = 0;

        [Header("Behaviour Overrides")]
        public bool overrideIsAutomatic = false;
        public bool isAutomaticValue = false;

        public bool overrideIsMolotov = false;
        public bool isMolotovValue = false;

        public float dotAreaRadiusBonus = 0f;
        public float dotDamageBonus = 0f;
        public float dotDurationBonus = 0f;
        public float dotIntervalBonus = 0f;
        public float dotAreaLifetimeBonus = 0f;

        [Header("Weight")]
        // Percent change to weapon weight (e.g. 0.2 = +20%, -0.15 = -15%)
        public float weightPercentBonus = 0f;
    }
}

