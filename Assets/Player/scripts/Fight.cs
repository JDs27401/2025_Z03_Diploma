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

        // Melee attacks are driven by MeleeWeaponItem; legacy melee removed

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

            if (weaponController.currentWeapon != null || weaponController.currentMeleeWeapon != null)
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