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
        private float _waitUntilDestroyed = 0f;

        private Actor a;
        
        private bool _triggered = false;
        
        private Animator animator;
        

        private void Start()
        {
            if (ReferenceEquals(trapTrigger, null) || ReferenceEquals(damageTrigger, null))
            {
                return;
            }

            a = GetComponent<Actor>();
            if (ReferenceEquals(a, null))
            {
                a = GetComponentInParent<Actor>();
            }

            if (!ReferenceEquals(a, null))
            {
                _waitUntilDestroyed = a.GetWaitUntilDestroyed();
            }
            
            trapTrigger.radius = trapTriggerRadius;
            damageTrigger.enabled = false;

            if (GetComponent<ExplodingEnemy>())
            {
                animator = a.GetComponent<Animator>();
            }
            else
            {
                animator = GetComponent<Animator>();
            }
            
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered || other.CompareTag("projectile") || other.CompareTag("trap")|| ReferenceEquals(other.gameObject.GetComponent<Actor>(), null))
            {
                yield break;
            }
            
            _triggered = true;
            print("start");
            
            
            if (GetComponent<ExplodingEnemy>()) animator.SetTrigger("AboutToExplode");
            
            
            yield return new WaitForSeconds(explodeAfter);
            
            animator.SetTrigger("Explode");
            
            print("end");
            tag = "trap";
            damageTrigger.enabled = true;
            damageTrigger.radius = damageTriggerRadius;
            
            // print("boom"); //just a check if the mine works
            // yield return new WaitForFixedUpdate();
            Destroy(damageTrigger, destroyTriggerAfter);
            Destroy(gameObject, _waitUntilDestroyed);
        }
    }
}