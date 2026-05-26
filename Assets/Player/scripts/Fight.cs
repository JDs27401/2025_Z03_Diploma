using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using C__Classes; // Potrzebne do dostępu do Actor
using C__Classes.Managers;

namespace Player.scripts
{
    public class Fight : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform firePoint; // Punkt wylotu pocisków/ataku
        
        [Header("Weapons")]
        [Tooltip("Lista broni dostępnych dla gracza")]
        public WeaponData[] availableWeapons;
        [Tooltip("Punkt do którego będzie przyczepiany model broni (np. Ramię)")]
        public Transform weaponHolder;
        private WeaponController weaponController;
        private int currentWeaponIndex = 0;
        [SerializeField] private bool useInventoryWeaponSelection = true;

        [Header("Melee (Legayc)")]
        [Tooltip("Prefab ataku wręcz. Musi posiadać Collider2D(Trigger), Actor, DamagePipeline oraz tag 'attack'.")]
        [SerializeField] private GameObject meleeHitboxPrefab;

        [Header("Melee Stats")]
        [SerializeField] private float meleeSpeed = 2f; // ataki na sekundę
        [SerializeField] private float meleeDamage = 25f;
        [SerializeField] private float meleeDuration = 1f; // Jak długo hitbox istnieje na scenie
        [SerializeField] private float meleeAngle = 120;
        [SerializeField] private float meleeRange = 1f;

        [Header("Ranged Stats")]
        [SerializeField] private float shootingSpeed = 4f; // strzały na sekundę
        [SerializeField] private float projectileSpeed = 10f;
        private float projectileDamage = 10f;
        [SerializeField] private float projectileSpread = 5f; // rozrzut w stopniach
        

        private float nextMeleeTime = 0f;

        private Camera mainCam;

        void Start()
        {
            mainCam = Camera.main;
            if (mainCam == null) mainCam = FindFirstObjectByType<Camera>();
            
            if (firePoint == null) firePoint = transform;
            if (weaponHolder == null) weaponHolder = transform; 

            weaponController = GetComponent<WeaponController>();
            if (weaponController == null)
            {
                weaponController = gameObject.AddComponent<WeaponController>();
            }
            weaponController.firePoint = this.firePoint;
            weaponController.weaponHolder = this.weaponHolder; 

            if (useInventoryWeaponSelection && GetComponent<WeaponInventoryBridge>() == null)
            {
                gameObject.AddComponent<WeaponInventoryBridge>();
            }

            if (!useInventoryWeaponSelection && availableWeapons != null && availableWeapons.Length > 0)
            {
                EquipWeapon(0);
            }
        }

        void Update()
        {
            if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
            if (Mouse.current == null) return;

            Vector3 mouseWorldPos = GetMouseWorldPosition();
            
            if (weaponController != null)
            {
                weaponController.AimAt(mouseWorldPos);
            }

            if (!useInventoryWeaponSelection)
            {
                Vector2 scroll = Mouse.current.scroll.ReadValue();
                if (scroll.y > 0)
                {
                    SwitchWeapon(1);
                }
                else if (scroll.y < 0)
                {
                    SwitchWeapon(-1);
                }
            }

            if (weaponController.currentWeapon != null)
            {
                bool wantsToShoot = false;
                if (weaponController.CurrentWeaponIsAutomatic)
                {
                    wantsToShoot = Mouse.current.leftButton.isPressed;
                }
                else
                {
                    wantsToShoot = Mouse.current.leftButton.wasPressedThisFrame;
                }

                if (wantsToShoot)
                {
                    Vector2 direction = (mouseWorldPos - firePoint.position).normalized;
                    weaponController.TryShoot(direction);
                }
            }


            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Time.time >= nextMeleeTime)
                {
                    MeleeAttack();
                    nextMeleeTime = Time.time + (1f / meleeSpeed);
                }
            }
        }

        void MeleeAttack()
        {
            if (meleeHitboxPrefab == null) return;

            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Vector2 direction = (mouseWorldPos - firePoint.position).normalized;
            float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            Quaternion rotation = Quaternion.Euler(0, 0, rotZ);

            GameObject hitbox = Instantiate(meleeHitboxPrefab, firePoint.position, rotation);
            hitbox.transform.SetParent(this.transform);

            ArcHitbox arcScript = hitbox.GetComponent<ArcHitbox>();
            if (arcScript != null)
            {
                arcScript.SetArcShape(meleeAngle, meleeRange);
            }

            Actor actorScript = hitbox.GetComponent<Actor>();
            if (actorScript != null)
            {
                actorScript.SetDamage(meleeDamage);
            }

            Destroy(hitbox, meleeDuration);
        }

        private void SwitchWeapon(int dir)
        {
            if (availableWeapons == null || availableWeapons.Length == 0) return;

            currentWeaponIndex += dir;
            if (currentWeaponIndex >= availableWeapons.Length) currentWeaponIndex = 0;
            if (currentWeaponIndex < 0) currentWeaponIndex = availableWeapons.Length - 1;

            EquipWeapon(currentWeaponIndex);
        }

        private void EquipWeapon(int index)
        {
            if (weaponController != null)
            {
                weaponController.EquipWeapon(availableWeapons[index]);
                Debug.Log("Wybrano broń: " + availableWeapons[index].weaponName);
            }
        }


        Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
            return mainCam.ScreenToWorldPoint(mouseScreenPos);
        }
    }
}