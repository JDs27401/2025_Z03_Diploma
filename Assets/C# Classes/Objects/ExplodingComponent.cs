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

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered || other.CompareTag("projectile") || other.CompareTag("trap")|| ReferenceEquals(other.gameObject.GetComponent<Actor>(), null))
            {
                yield break;
            }
            
            _triggered = true;
            #if UNITY_EDITOR
            print("start");
            #endif
            if (GetComponent<ExplodingEnemy>())
            {
                _animator.SetTrigger("AboutToExplode");
            }
            
            yield return new WaitForSeconds(explodeAfter);
            #if UNITY_EDITOR
            print("end");
            #endif
            _animator.SetTrigger("Explode");
            
            tag = "trap";
            damageTrigger.enabled = true;
            damageTrigger.radius = damageTriggerRadius;
            
            // print("boom"); //just a check if the mine works
            // yield return new WaitForFixedUpdate();
            Destroy(damageTrigger, destroyTriggerAfter);
            Destroy(gameObject, _actor.GetWaitUntilDestroyed());
        }
    }
}