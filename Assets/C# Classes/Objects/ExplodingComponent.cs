using System.Collections;
using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Objects
{
    public class ExplodingComponent : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D trapTrigger;
        [SerializeField] private float trapTriggerRadius = 0.5f;
        
        [SerializeField] private CircleCollider2D damageTrigger;
        [SerializeField] private float damageTriggerRadius = 3f;

        [SerializeField] private float explodeAfter = 0f;

        [SerializeField] private float destroyTriggerAfter = 0.25f;

        private Actor _actor;
        private bool _triggered = false;
        
        private Animator _animator;
        
        private void Start()
        {
            if (ReferenceEquals(trapTrigger, null) || ReferenceEquals(damageTrigger, null))
            {
                return;
            }

            _actor = GetComponent<Actor>();
            if (ReferenceEquals(_actor, null))
            {
                _actor = GetComponentInParent<Actor>();
            }

            if (ReferenceEquals(_actor, null))
            {
                return;
            }
            
            trapTrigger.radius = trapTriggerRadius;
            damageTrigger.enabled = false;

            _animator = GetComponent<Animator>();
            if (ReferenceEquals(_animator, null))
            {
                #if UNITY_EDITOR 
                print("Animator not found");
                #endif
                return;
            }
        }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (damageTrigger.enabled)
        {
            return;
        }
        
        if (other.gameObject == gameObject) return;
        
        HandleTrapTrigger(other);
    }

    private void HandleTrapTrigger(Collider2D other)
    {
        
        if (!other.CompareTag("hostile"))
        {
            return;
        }
        
        if (_triggered || other.CompareTag("projectile") || other.CompareTag("trap") || ReferenceEquals(other.gameObject.GetComponent<Actor>(), null))
        {
            return;
        }

        if (other.CompareTag("player") || other.CompareTag("Player"))
        {
            return;
        }
        
        _triggered = true;
        
        StopProjectile();
        
        #if UNITY_EDITOR
        #endif
        
        StartCoroutine(ExplodeCoroutine());
    }

    private void StopProjectile()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        Projectile proj = GetComponent<Projectile>();
        if (proj != null)
        {
            System.Type projType = proj.GetType();
            projType.GetField("hasCollided", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(proj, true);
        }
    }


    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(explodeAfter);
        
        #if UNITY_EDITOR
        #endif
        
        if (!ReferenceEquals(_animator, null))
        {
            _animator.SetTrigger("Explode");
        }
        
        tag = "trap";
        damageTrigger.enabled = true;
        damageTrigger.radius = damageTriggerRadius;
        
        Destroy(damageTrigger, destroyTriggerAfter);
        Destroy(gameObject, _actor.GetWaitUntilDestroyed());
    }

    }
}