using UnityEngine;
using C__Classes;
using UnityEngine.InputSystem;
using System;

namespace Player.scripts
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponData currentWeapon;
        public MeleeWeaponData currentMeleeWeapon;
        public Transform firePoint;
        public Transform weaponHolder;

        private GameObject _currentWeaponModel;
        private float _nextFireTime;
        private float _nextMeleeTime;

        // Przeładowanie
        private WeaponInstanceState _currentWeaponState;
        private bool _isReloading;
        private float _reloadTimer;
        private MeleeWeaponInstanceState _currentMeleeState;

        private bool _weaponDebug = true;
        private int _attacksLayer = -1;
        
        //animacje
        private PlayerController _playerController;
        private PlayerInput _playerInput;

        public event Action OnWeaponFired;

        public WeaponRuntimeStats CurrentWeaponStats
        {
            get
            {
                if (_currentWeaponState == null)
                {
                    return null;
                }

                return _currentWeaponState.GetRuntimeStats();
            }
        }

        public MeleeWeaponRuntimeStats CurrentMeleeStats
        {
            get
            {
                if (_currentMeleeState == null) return null;
                return _currentMeleeState.GetRuntimeStats();
            }
        }

        public bool CurrentWeaponIsAutomatic
        {
            get
            {
                WeaponRuntimeStats stats = CurrentWeaponStats;
                if (stats != null)
                {
                    return stats.isAutomatic;
                }

                if (currentWeapon != null && currentWeapon.isAutomatic)
                {
                    return true;
                }

                MeleeWeaponRuntimeStats mstats = CurrentMeleeStats;
                if (mstats != null)
                {
                    return mstats.isAutomatic;
                }

                return false; // default false if nothing
            }
        }

        private void Awake()
        {
            _attacksLayer = LayerMask.NameToLayer("Attacks");
            
            _playerController = GetComponent<PlayerController>();
            _playerInput = GetComponent<PlayerInput>();
            
            if (_playerInput != null)
            {
                var playerMap = _playerInput.actions.FindActionMap("Player");
                if (playerMap != null)
                {
                    var toggleLightsAction = playerMap.FindAction("ToggleLights");
                    if (toggleLightsAction != null)
                    {
                        toggleLightsAction.performed += OnToggleLights;
                    }
                }
            }
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private void Update()
        {
            if (_isReloading && currentWeapon != null)
            {
                WeaponRuntimeStats stats = CurrentWeaponStats;
                _reloadTimer -= Time.deltaTime;
                if (_reloadTimer <= 0f)
                {
                    _isReloading = false;
                    EnsureCurrentStateInitialized();
                    if (_currentWeaponState != null && stats != null)
                    {
                        _currentWeaponState.currentMagazineAmmo = stats.magazineSize;
                    }
                    if(_weaponDebug && stats != null) Debug.Log($"Przeładowano {stats.weaponName}! (Magazynek powraca do {stats.magazineSize})");
                }
            }
        }

        public void EquipWeapon(WeaponData newWeapon)
        {
            EquipWeapon(newWeapon, null);
        }

        public void EquipWeapon(WeaponData newWeapon, WeaponInstanceState state)
        {
            // When equipping ranged weapon, clear melee and set ranged
            _currentMeleeState = null;
            currentMeleeWeapon = null;

            _currentWeaponState = state;
            EquipWeaponInternal(newWeapon);
        }

        public void EquipMeleeWeapon(MeleeWeaponData newMelee, MeleeWeaponInstanceState state)
        {
            // When equipping melee, clear ranged
            _currentWeaponState = null;
            currentWeapon = null;

            _currentMeleeState = state;
            // configure visuals / animation similar to EquipWeapon
            _isReloading = false;
            _reloadTimer = 0f;

            currentMeleeWeapon = newMelee;

            if (_playerController != null)
            {
                int animID = (currentMeleeWeapon != null) ? currentMeleeWeapon.animationID : 0;
                _playerController.SetWeaponAnimation(animID);
            }

            EnsureCurrentStateInitialized();

            if (_currentWeaponModel != null)
            {
                Destroy(_currentWeaponModel);
                _currentWeaponModel = null;
            }

            if (currentMeleeWeapon != null && currentMeleeWeapon.weaponModelPrefab != null && weaponHolder != null)
            {
                _currentWeaponModel = Instantiate(currentMeleeWeapon.weaponModelPrefab, weaponHolder.position, weaponHolder.rotation);
                _currentWeaponModel.transform.SetParent(weaponHolder);
                _currentWeaponModel.transform.localPosition = Vector3.zero;
                _currentWeaponModel.transform.localRotation = Quaternion.identity;
            }
        }

        private void EquipWeaponInternal(WeaponData newWeapon)
        {
            _isReloading = false;
            _reloadTimer = 0f;

            currentWeapon = newWeapon;

            if (_playerController != null)
            {
                int animID = (currentWeapon != null) ? currentWeapon.animationID : 0;
                _playerController.SetWeaponAnimation(animID);
            }

            EnsureCurrentStateInitialized();

            if (_currentWeaponModel != null)
            {
                Destroy(_currentWeaponModel);
                _currentWeaponModel = null;
            }

            if (currentWeapon != null && currentWeapon.weaponModelPrefab != null && weaponHolder != null)
            {
                _currentWeaponModel = Instantiate(currentWeapon.weaponModelPrefab, weaponHolder.position, weaponHolder.rotation);
                _currentWeaponModel.transform.SetParent(weaponHolder);
                
                _currentWeaponModel.transform.localPosition = Vector3.zero;
                _currentWeaponModel.transform.localRotation = Quaternion.identity;
            }
        }

        private void EnsureCurrentStateInitialized()
        {
            // Ranged
            if (currentWeapon == null)
            {
                _currentWeaponState = null;
            }
            else
            {
                if (_currentWeaponState == null)
                {
                    _currentWeaponState = new WeaponInstanceState(currentWeapon.magazineSize);
                }

                _currentWeaponState.InitializeFromWeaponData(currentWeapon);

                if (_currentWeaponState.runtimeStats != null)
                {
                    _currentWeaponState.currentMagazineAmmo = Mathf.Clamp(
                        _currentWeaponState.currentMagazineAmmo,
                        0,
                        _currentWeaponState.runtimeStats.magazineSize
                    );
                }
            }

            // Melee
            if (currentMeleeWeapon == null)
            {
                _currentMeleeState = null;
            }
            else
            {
                if (_currentMeleeState == null)
                {
                    _currentMeleeState = new MeleeWeaponInstanceState();
                }

                _currentMeleeState.InitializeFromMeleeData(currentMeleeWeapon);
            }
        }

        public void AimAt(Vector2 targetPos)
        {
            if (weaponHolder != null)
            {
                Vector2 direction = (targetPos - (Vector2)weaponHolder.position).normalized;
                float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                weaponHolder.rotation = Quaternion.Euler(0, 0, rotZ);

                if (rotZ > 90 || rotZ < -90)
                    weaponHolder.localScale = new Vector3(1, -1, 1);
                else
                    weaponHolder.localScale = new Vector3(1, 1, 1);
            }
        }

        private void StartReload()
        {
            WeaponRuntimeStats stats = CurrentWeaponStats;
            if (stats == null)
            {
                return;
            }

            _isReloading = true;
            _reloadTimer = stats.reloadTime;
            if(_weaponDebug) Debug.Log($"reloading ({stats.reloadTime})");
        }

        public void OnToggleLights(InputAction.CallbackContext context)
        {
            if (currentWeapon == null || _currentWeaponState == null)
            {
                if (_weaponDebug) Debug.Log("Nie można włączyć światła - brak wyekwipowanej broni");
                return;
            }

            bool hasFlashlight = _currentWeaponState.HasModType(WeaponModType.Flashlight);
            bool hasLaserSight = _currentWeaponState.HasModType(WeaponModType.LaserSight);

            if (!hasFlashlight && !hasLaserSight)
            {
                if (_weaponDebug) Debug.Log("Nie można włączyć światła - brak zainstalowanych modów");
                return;
            }

            if (hasFlashlight)
            {
                WeaponModInstanceState flashlight = _currentWeaponState.GetModByType(WeaponModType.Flashlight);
                if (flashlight != null)
                {
                    flashlight.isActive = !flashlight.isActive;
                    if (_weaponDebug) Debug.Log($"Latarka: {(flashlight.isActive ? "WŁĄCZONA" : "WYŁĄCZONA")}");
                }
            }

            if (hasLaserSight)
            {
                WeaponModInstanceState laser = _currentWeaponState.GetModByType(WeaponModType.LaserSight);
                if (laser != null)
                {
                    laser.isActive = !laser.isActive;
                    if (_weaponDebug) Debug.Log($"Laser: {(laser.isActive ? "WŁĄCZONY" : "WYŁĄCZONY")}");
                }
            }
        }

        private void ConfigureProjectile(GameObject bullet, WeaponRuntimeStats weaponSettings)
        {
            if (weaponSettings == null)
            {
                return;
            }

            Actor actorScript = bullet.GetComponent<Actor>();
            if (actorScript == null) 
            {
                actorScript = bullet.AddComponent<Actor>();
            }
            actorScript.SetDamage(weaponSettings.isMolotov ? 0f : weaponSettings.damage);

            bullet.tag = "projectile";
            
            if (weaponSettings.isExplosive)
            {
                
                CircleCollider2D trapTrigger = bullet.AddComponent<CircleCollider2D>();
                trapTrigger.isTrigger = true;
                trapTrigger.radius = 0.1f;

                CircleCollider2D damageTrigger = bullet.AddComponent<CircleCollider2D>();
                damageTrigger.isTrigger = true;
                damageTrigger.enabled = false;

                C__Classes.Objects.ExplodingComponent explodingComp = bullet.GetComponent<C__Classes.Objects.ExplodingComponent>();
                if (explodingComp == null) explodingComp = bullet.AddComponent<C__Classes.Objects.ExplodingComponent>();

                System.Type type = explodingComp.GetType();
                
                type.GetField("trapTrigger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(explodingComp, trapTrigger);
                    
                type.GetField("damageTrigger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(explodingComp, damageTrigger);
                
                type.GetField("damageTriggerRadius", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(explodingComp, weaponSettings.explosionRadius);
                
                type.GetField("explodeAfter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(explodingComp, 0f); // Wymuszenie natychmiastowego wybuchu
                
                type.GetField("destroyTriggerAfter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(explodingComp, 0.25f);
                
                actorScript.SetWaitUntilDestroyed(0.25f);
            }
        }

        public void TryShoot(Vector2 targetDirection)
        {
            if (firePoint == null) return;

            EnsureCurrentStateInitialized();

            // If melee is equipped -> perform melee attack
            if (currentMeleeWeapon != null)
            {
                TryMeleeAttack(targetDirection);
                return;
            }

            // Otherwise fallback to ranged
            if (currentWeapon == null) return;

            WeaponRuntimeStats weaponStats = CurrentWeaponStats;
            if (weaponStats == null) return;
            if (_isReloading) return;
            if (_currentWeaponState.currentMagazineAmmo <= 0)
            {
                StartReload();
                return;
            }

            if (Time.time < _nextFireTime) return;

            _nextFireTime = Time.time + (1f / weaponStats.fireRate);

            _currentWeaponState.currentMagazineAmmo--;
            if(_weaponDebug) Debug.Log($"Ammo: {_currentWeaponState.currentMagazineAmmo}");

            if (_currentWeaponState.currentMagazineAmmo == 0)
            {
                StartReload();
            }

            OnWeaponFired?.Invoke();

            for (int i = 0; i < weaponStats.projectilesPerShot; i++)
            {
                float rotZ = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                float spread = weaponStats.spread;
                if (_playerController != null)
                {
                    spread *= _playerController.GetCachedWeaponSpreadMultiplier();
                }
                float randomSpread = UnityEngine.Random.Range(-spread, spread);
                Quaternion rotation = Quaternion.Euler(0, 0, rotZ + randomSpread);
                GameObject bullet = Instantiate(currentWeapon.projectilePrefab, firePoint.position, rotation);

                if (_attacksLayer >= 0)
                {
                    SetLayerRecursively(bullet, _attacksLayer);
                }
                
                Projectile proj = bullet.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Setup(weaponStats);
                }
                
                ConfigureProjectile(bullet, weaponStats);

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = bullet.transform.right * weaponStats.projectileSpeed;
                }
            }
        }

        private void TryMeleeAttack(Vector2 targetDirection)
        {
            EnsureCurrentStateInitialized();
            MeleeWeaponRuntimeStats meleeStats = CurrentMeleeStats;
            if (meleeStats == null) return;

            if (Time.time < _nextMeleeTime) return;
            if (!_playerController || _playerController.GetStamina() < 30 || _playerController.GetStamina() < meleeStats.staminaCost)
            {
                return;
            }

            _nextMeleeTime = Time.time + (1f / meleeStats.attackRate);

            //OnWeaponFired?.Invoke();

            // Spawn hitbox prefab
            if (currentMeleeWeapon == null || currentMeleeWeapon.hitboxPrefab == null) return;

            float rotZ = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, rotZ);
            GameObject hitbox = Instantiate(currentMeleeWeapon.hitboxPrefab, firePoint.position, rotation);

            if (_attacksLayer >= 0)
            {
                SetLayerRecursively(hitbox, _attacksLayer);
            }

            ArcHitbox arcScript = hitbox.GetComponent<ArcHitbox>();
            if (arcScript != null)
            {
                arcScript.SetArcShape(meleeStats.angle, meleeStats.range);
            }
            Actor actorScript = hitbox.GetComponent<Actor>();
            if (actorScript != null)
            {
                actorScript.SetDamage(meleeStats.damage);
            }

            // Subtract stamina cost from player (if player controller available)
            try
            {
                if (_playerController != null)
                {
                    _playerController.ReduceStamina(meleeStats.staminaCost);
                }
            }
            catch (System.Exception ex)
            {
                if (_weaponDebug) Debug.LogWarning($"Failed to reduce player stamina: {ex.Message}");
            }

            Destroy(hitbox, meleeStats.hitboxDuration);
        }

        private void OnDestroy()
        {
            if (_playerInput != null)
            {
                var playerMap = _playerInput.actions.FindActionMap("Player");
                if (playerMap != null)
                {
                    var toggleLightsAction = playerMap.FindAction("ToggleLights");
                    if (toggleLightsAction != null)
                    {
                        toggleLightsAction.performed -= OnToggleLights;
                    }
                }
            }
        }
    }
}