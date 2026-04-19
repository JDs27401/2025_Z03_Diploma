using UnityEngine;
using C__Classes;
using System.Collections.Generic;

namespace Player.scripts
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponData currentWeapon;
        public Transform firePoint;
        public Transform weaponHolder;

        private GameObject _currentWeaponModel;
        private float _nextFireTime;

        // Przeładowanie
        private Dictionary<string, int> _ammoState = new Dictionary<string, int>();
        private bool _isReloading;
        private float _reloadTimer;

        private bool _weaponDebug = true;
        private int _attacksLayer = -1;

        private void Awake()
        {
            _attacksLayer = LayerMask.NameToLayer("Attacks");
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
                _reloadTimer -= Time.deltaTime;
                if (_reloadTimer <= 0f)
                {
					_isReloading = false;
					_ammoState[currentWeapon.weaponName] = currentWeapon.magazineSize;
          if(_weaponDebug) Debug.Log($"Przeładowano {currentWeapon.weaponName}! (Magazynek powraca do {currentWeapon.magazineSize})");
                }
            }
        }

        public void EquipWeapon(WeaponData newWeapon)
        {
            _isReloading = false;
            _reloadTimer = 0f;

            currentWeapon = newWeapon;

            if (currentWeapon != null && !_ammoState.ContainsKey(currentWeapon.weaponName))
            {
                _ammoState[currentWeapon.weaponName] = currentWeapon.magazineSize;
            }

            if (_currentWeaponModel != null)
            {
                Destroy(_currentWeaponModel);
            }

            if (currentWeapon != null && currentWeapon.weaponModelPrefab != null && weaponHolder != null)
            {
                _currentWeaponModel = Instantiate(currentWeapon.weaponModelPrefab, weaponHolder.position, weaponHolder.rotation);
                _currentWeaponModel.transform.SetParent(weaponHolder);
                
                _currentWeaponModel.transform.localPosition = Vector3.zero;
                _currentWeaponModel.transform.localRotation = Quaternion.identity;
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

        // public void TargetReload() 
        // {
        //     if (currentWeapon == null || isReloading) return;
        //
        //     if (ammoState[currentWeapon.weaponName] >= currentWeapon.magazineSize) return;
        //
        //     StartReload();
        // }

        private void StartReload()
        {
            _isReloading = true;
            _reloadTimer = currentWeapon.reloadTime;
            if(_weaponDebug) Debug.Log($"reloading ({currentWeapon.reloadTime})");
        }

        private void ConfigureProjectile(GameObject bullet, WeaponData weaponSettings)
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
            if (currentWeapon == null || firePoint == null) return;
            if (_isReloading) return;
            if (_ammoState[currentWeapon.weaponName] <= 0)
            {
                StartReload();
                return;
            }
            if (Time.time < _nextFireTime) return;

            _nextFireTime = Time.time + (1f / currentWeapon.fireRate);
            
            _ammoState[currentWeapon.weaponName]--;
            if(_weaponDebug) Debug.Log($"Ammo: {_ammoState[currentWeapon.weaponName]}");
            
            if (_ammoState[currentWeapon.weaponName] == 0)
            {
                StartReload();
            }

            for (int i = 0; i < currentWeapon.projectilesPerShot; i++)
            {
                float rotZ = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                float randomSpread = Random.Range(-currentWeapon.spread, currentWeapon.spread);
                Quaternion rotation = Quaternion.Euler(0, 0, rotZ + randomSpread);
                GameObject bullet = Instantiate(currentWeapon.projectilePrefab, firePoint.position, rotation);

                if (_attacksLayer >= 0)
                {
                    SetLayerRecursively(bullet, _attacksLayer);
                }
                
                Projectile proj = bullet.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Setup(currentWeapon);
                }
                
                ConfigureProjectile(bullet, currentWeapon);

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = bullet.transform.right * currentWeapon.projectileSpeed;
                }
            }
        }
    }
}