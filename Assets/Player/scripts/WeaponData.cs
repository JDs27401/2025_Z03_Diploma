namespace Player.scripts
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "New Weapon";
        public int animationID;
    
        [Header("Shooting Stats")]
        public float fireRate = 4f; // Strzały na sekundę
        public float damage = 10f;
        public float projectileSpeed = 15f;
        public float spread = 0f; // Rozrzut w stopniach
        public int projectilesPerShot = 1; // Strzelba potrzebuje więcej niż 1

        [Header("Mechanics")]
        public bool isAutomatic = false; // Czy można trzymać przycisk, aby strzelać ciągiem

        [Header("Ammo & Reloading")]
        public int magazineSize = 10; // Ilość nabojów w magazynku
        public float reloadTime = 2f; // Czas przeładowania w sekundach

        [Header("Explosive (RPG)")]
        public bool isExplosive = false;
        public float explosionRadius = 3f;

        [Header("Molotov / DOT")]
        public bool isMolotov = false;
        public float dotAreaRadius = 2.5f;
        public float dotDamage = 4f;
        public float dotDuration = 4f;
        public float dotInterval = 1f;
        public float dotAreaLifetime = 5f;

        [Header("Visuals")]
        public GameObject weaponModelPrefab; // Model/Prefab broni wyswietlany w rece

        [Header("Prefabs")]
        public GameObject projectilePrefab;
    }

}