namespace Player.scripts
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapons/Melee Weapon Data")]
    public class MeleeWeaponData : ScriptableObject
    {
        public string weaponName = "New Melee Weapon";
        public int animationID;

        [Header("Melee Stats")]
        public float attackRate = 1.5f; // ataki na sekundę
        public float damage = 25f;
        public float range = 1.0f; // promień łuku hitboxa
        public float angle = 120f; // kąt rozwartości łuku (stopnie)
        public float hitboxDuration = 0.2f; // ile sekund hitbox istnieje
            [Header("Stamina")]
            [Tooltip("Ilość staminy zużywana przy każdym ataku tą bronią (liczba całkowita)")]
            public int staminaCost = 5;

        [Header("Visuals")]
        public GameObject weaponModelPrefab;

        [Header("Prefabs")]
        public GameObject hitboxPrefab; // prefab zawierający ArcHitbox + Actor

        [Header("Physics")]
        public float weight = 1.0f;

        [Header("Molotov / DOT")]
        public bool isMolotov = false;
        public float dotAreaRadius = 2.5f;
        public float dotDamage = 4f;
        public float dotDuration = 4f;
        public float dotInterval = 1f;
        public float dotAreaLifetime = 5f;
    }
}

