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
            Normalize();
        }

        public void Normalize()
        {
            attackRate = Mathf.Max(0.01f, attackRate);
            damage = Mathf.Max(0f, damage);
            range = Mathf.Max(0f, range);
            angle = Mathf.Clamp(angle, 0f, 360f);
            hitboxDuration = Mathf.Max(0f, hitboxDuration);
            weight = Mathf.Max(0f, weight);
        }
    }
}

