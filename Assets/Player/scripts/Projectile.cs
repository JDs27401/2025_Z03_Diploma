using C__Classes; // Do dostępu do klasy Actor
using UnityEngine;
using C__Classes.Objects;
namespace Player.scripts
{
    public class Projectile : MonoBehaviour
    {
        private WeaponRuntimeStats weaponSettings;
        private float _speed;
        private bool _isExplosive;
        private bool isMolotov;
        private float _explosionRadius;
        private float dotAreaRadius;
        private float dotDamage;
        private float dotDuration;
        private float dotInterval;
        private float dotAreaLifetime;
        private bool _hasCollided;
        private Vector3 currentSpeed;
        private float angularSpeed = 0;
        private Rigidbody2D rb;

        public void Setup(WeaponRuntimeStats settings)
        {
            weaponSettings = settings;

            if (weaponSettings == null)
            {
                return;
            }

            _speed = weaponSettings.projectileSpeed;
            _isExplosive = weaponSettings.isExplosive;
            isMolotov = weaponSettings.isMolotov;
            _explosionRadius = weaponSettings.explosionRadius;
            dotAreaRadius = weaponSettings.dotAreaRadius;
            dotDamage = weaponSettings.dotDamage;
            dotDuration = weaponSettings.dotDuration;
            dotInterval = weaponSettings.dotInterval;
            dotAreaLifetime = weaponSettings.dotAreaLifetime;
            rb = GetComponent<Rigidbody2D>();
            if (settings.ammoType == AmmoType.Consumable)
            {
                angularSpeed = 360;
            }
            else
            {
                angularSpeed = 0;
            }
            
            if (rb)
            {
                rb.linearVelocity = transform.right * _speed;
                rb.angularVelocity = angularSpeed;
            }
            
            Destroy(gameObject, 5f);
        }

        void Update()
        {
            if (_hasCollided)
            {
                return;
            }
        }

        private void StopProjectile()
        {
            _hasCollided = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_isExplosive)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, _explosionRadius);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollision(other);
        }

        private void HandleCollision(Collider2D other)
        {
            if (_hasCollided) return;

            if (other.CompareTag("hostile") || other.CompareTag("destructible"))
            {
                StopProjectile();

                if (_isExplosive && GetComponent<ExplodingComponent>() != null)
                {
                    return;
                }

                if (isMolotov)
                {
                    SpawnDotArea(other.ClosestPoint(transform.position));
                }

                Destroy(gameObject);
            }
            
        }

        private void SpawnDotArea(Vector3 position)
        {
            GameObject dotArea = new GameObject("MolotovDotArea");
            dotArea.transform.position = position;
            dotArea.layer = LayerMask.NameToLayer("Attacks");
            
            Rigidbody2D rb = dotArea.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            
            CircleCollider2D trigger = dotArea.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = dotAreaRadius;

            Actor areaActor = dotArea.AddComponent<Actor>();
            areaActor.SetDamage(dotDamage);
            areaActor.SetWaitUntilDestroyed(dotAreaLifetime);

            DotComponent dotComponent = dotArea.AddComponent<DotComponent>();
            dotComponent.Configure(trigger, dotAreaRadius, dotDuration, dotInterval);
            dotComponent.StartDotArea();
        }

    }
    
}