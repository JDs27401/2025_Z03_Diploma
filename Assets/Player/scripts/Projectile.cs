using C__Classes; // Do dostępu do klasy Actor
using UnityEngine;
using C__Classes.Objects;
namespace Player.scripts
{
    public class Projectile : MonoBehaviour
    {
        private const string DetachedExplosionEffectName = "ExplosionEffect";
        private const string DetachedExplosionStateName = "Explode";
        private const float DetachedExplosionEffectLifetime = 1f;
        private static readonly int ExplodeTrigger = Animator.StringToHash("Explode");

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
        private Transform _detachedExplosionEffectTemplate;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _projectileCollider;

        public void Setup(WeaponRuntimeStats settings)
        {
            weaponSettings = settings;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _projectileCollider = GetComponent<BoxCollider2D>();
            CacheDetachedExplosionEffectTemplate();

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

                if (_isExplosive)
                {
                    SpawnDetachedExplosionEffect();
                }

                if (_isExplosive && GetComponent<ExplodingComponent>() != null)
                {
                    HideProjectileVisuals();
                    return;
                }

                if (isMolotov)
                {
                    SpawnDotArea(other.ClosestPoint(transform.position));
                }

                Destroy(gameObject);
            }

        }

        private void CacheDetachedExplosionEffectTemplate()
        {
            if (_detachedExplosionEffectTemplate != null)
            {
                return;
            }

            _detachedExplosionEffectTemplate = transform.Find(DetachedExplosionEffectName);
            if (_detachedExplosionEffectTemplate == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponentInChildren<Animator>(true) != null)
                    {
                        _detachedExplosionEffectTemplate = child;
                        break;
                    }
                }
            }

            if (_detachedExplosionEffectTemplate != null)
            {
                _detachedExplosionEffectTemplate.gameObject.SetActive(false);
            }
        }

        private void SpawnDetachedExplosionEffect()
        {
            if (_detachedExplosionEffectTemplate == null)
            {
                return;
            }

            GameObject spawnedEffect = Instantiate(
                _detachedExplosionEffectTemplate.gameObject,
                transform.position,
                _detachedExplosionEffectTemplate.rotation);

            
            spawnedEffect.name = _detachedExplosionEffectTemplate.gameObject.name;
            spawnedEffect.transform.localScale = _detachedExplosionEffectTemplate.lossyScale;
            spawnedEffect.SetActive(true);

            AudioSource effectAudioSource = spawnedEffect.GetComponentInChildren<AudioSource>(true);
            if (effectAudioSource != null)
            {
                effectAudioSource.pitch = Random.Range(0.9f, 1.1f);
                effectAudioSource.volume = 0.2f;
                effectAudioSource.Play();
            }
            
            foreach (Collider2D childCollider in spawnedEffect.GetComponentsInChildren<Collider2D>(true))
            {
                childCollider.enabled = false;
            }

            foreach (Rigidbody2D childRigidbody in spawnedEffect.GetComponentsInChildren<Rigidbody2D>(true))
            {
                childRigidbody.linearVelocity = Vector2.zero;
                childRigidbody.angularVelocity = 0f;
                childRigidbody.simulated = false;
            }

            Animator effectAnimator = spawnedEffect.GetComponentInChildren<Animator>(true);
            if (effectAnimator != null)
            {
                effectAnimator.Rebind();
                effectAnimator.Update(0f);
                effectAnimator.Play(DetachedExplosionStateName, 0, 0f);
                effectAnimator.Update(0f);
            }
            else
            {
                Animator fallbackAnimator = spawnedEffect.GetComponent<Animator>();
                if (fallbackAnimator != null)
                {
                    fallbackAnimator.Rebind();
                    fallbackAnimator.Update(0f);
                    fallbackAnimator.Play(DetachedExplosionStateName, 0, 0f);
                    fallbackAnimator.Update(0f);
                }
            }

            Destroy(spawnedEffect, DetachedExplosionEffectLifetime);
        }

        private void HideProjectileVisuals()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = false;
            }

            if (_projectileCollider != null)
            {
                _projectileCollider.enabled = false;
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