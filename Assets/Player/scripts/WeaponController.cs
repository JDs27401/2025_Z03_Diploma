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

        private GameObject currentWeaponModel;
        private float nextFireTime = 0f;

        // Przeładowanie
        private Dictionary<string, int> ammoState = new Dictionary<string, int>();
        private bool isReloading = false;
        private float reloadTimer = 0f;

        private bool weaponDebug = true;

        private void Update()
        {
            if (isReloading && currentWeapon != null)
            {
                reloadTimer -= Time.deltaTime;
                if (reloadTimer <= 0f)
                {
                    isReloading = false;
                    ammoState[currentWeapon.weaponName] = currentWeapon.magazineSize;
					if(weaponDebug) Debug.Log($"Przeładowano {currentWeapon.weaponName}! (Magazynek powraca do {currentWeapon.magazineSize})");
                }
            }
        }

        public void EquipWeapon(WeaponData newWeapon)
        {
            isReloading = false;
            reloadTimer = 0f;

            currentWeapon = newWeapon;

            if (currentWeapon != null && !ammoState.ContainsKey(currentWeapon.weaponName))
            {
                ammoState[currentWeapon.weaponName] = currentWeapon.magazineSize;
            }

            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }

            if (currentWeapon != null && currentWeapon.weaponModelPrefab != null && weaponHolder != null)
            {
                currentWeaponModel = Instantiate(currentWeapon.weaponModelPrefab, weaponHolder.position, weaponHolder.rotation);
                currentWeaponModel.transform.SetParent(weaponHolder);
                
                currentWeaponModel.transform.localPosition = Vector3.zero;
                currentWeaponModel.transform.localRotation = Quaternion.identity;
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
            isReloading = true;
            reloadTimer = currentWeapon.reloadTime;
            if(weaponDebug) Debug.Log($"reloading ({currentWeapon.reloadTime})");
        }

        private void ConfigureProjectile(GameObject bullet, WeaponData weaponSettings)
        {
            Actor actorScript = bullet.GetComponent<Actor>();
            if (actorScript == null) 
            {
                actorScript = bullet.AddComponent<Actor>();
            }
            actorScript.SetDamage(weaponSettings.damage);

            bullet.tag = "projectile";
            
            if (weaponSettings.isExplosive)
            {
                
                CircleCollider2D trapTrigger = bullet.AddComponent<CircleCollider2D>();
                trapTrigger.isTrigger = true;
                trapTrigger.radius = 0.1f;

                CircleCollider2D damageTrigger = bullet.AddComponent<CircleCollider2D>();
                damageTrigger.isTrigger = true;
                damageTrigger.enabled = false;

                Animator animator = bullet.GetComponent<Animator>();
                if (animator == null) animator = bullet.AddComponent<Animator>();

                C__Classes.Objects.ExplodingComponent explodingComp = bullet.GetComponent<C__Classes.Objects.ExplodingComponent>();
                if (explodingComp == null) explodingComp = bullet.AddComponent<C__Classes.Objects.ExplodingComponent>();

                C__Classes.Objects.ExplodingComponent explodingComponent = bullet.GetComponent<C__Classes.Objects.ExplodingComponent>();
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
                
                bullet.GetComponent<Actor>().SetWaitUntilDestroyed(0.25f);
            }
        }

        public void TryShoot(Vector2 targetDirection)
        {
            if (currentWeapon == null || firePoint == null) return;
            if (isReloading) return;
            if (ammoState[currentWeapon.weaponName] <= 0)
            {
                StartReload();
                return;
            }
            if (Time.time < nextFireTime) return;

            nextFireTime = Time.time + (1f / currentWeapon.fireRate);
            
            ammoState[currentWeapon.weaponName]--;
            if(weaponDebug) Debug.Log($"Ammo: {ammoState[currentWeapon.weaponName]}");
            
            if (ammoState[currentWeapon.weaponName] == 0)
            {
                StartReload();
            }

            for (int i = 0; i < currentWeapon.projectilesPerShot; i++)
            {
                float rotZ = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                float randomSpread = Random.Range(-currentWeapon.spread, currentWeapon.spread);
                Quaternion rotation = Quaternion.Euler(0, 0, rotZ + randomSpread);
                GameObject bullet = Instantiate(currentWeapon.projectilePrefab, firePoint.position, rotation);
                
                Projectile proj = bullet.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Setup(currentWeapon.projectileSpeed, currentWeapon.damage, 1f, currentWeapon.isExplosive, currentWeapon.explosionRadius);
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