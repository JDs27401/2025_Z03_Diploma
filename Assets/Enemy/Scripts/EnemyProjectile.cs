using C__Classes;
using UnityEngine;

namespace Enemy.Scripts
{
    public class EnemyProjectile : MonoBehaviour
    {
        private float damage;
        private float speed;
        private Vector3 targetPos;
        private Vector3 direction; // If moving linearly
        private bool isHoming = false; // Could extend later if needed

        public void Setup(float dmg, float spd)
        {
            damage = dmg;
            speed = spd;
            
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Foreground";
                sr.sortingOrder = 50; 
            }
            
            // Destroy after 5 seconds to prevent memory leaks
            Destroy(gameObject, 5f);
        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Hit enemy
            if (other.GetComponent<NpcBase>() != null)
            {
                return;
            }

            // Hit player
            if (other.CompareTag("Player") || other.CompareTag("player")) 
            {
                Actor actor = other.GetComponent<Actor>();
                if (actor != null)
                {
                    actor.DealDamage(damage);
                    Destroy(gameObject);
                }
            }
            //Hit other stuff
            else if (!other.isTrigger && !other.CompareTag("projectile") && !other.CompareTag("heal"))
            {
                 if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == 0) // Default layer
                 {
                     Destroy(gameObject);
                 }
            }
        }
    }
}

