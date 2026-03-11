using C__Classes;
using Enemy.Scripts;
using UnityEngine;

namespace Enemy.Scripts
{
    public class RangerEnemy : NpcBase
    {
        [Header("Ranger Combat Stats")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileDamage = 10f;
        [SerializeField] private float fireRate = 1.5f; // Seconds between shots
        [SerializeField] private float attackRange = 10f;
        
        [Header("Ranger Movement")]
        [SerializeField] private float idealDistance = 6f; // Trying to keep this distance
        [SerializeField] private float distanceBuffer = 1f; // Doesn't jitter movement if within ideal +/- buffer
        
        private float _fireCooldown = 0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            // Handle shooting cooldown here (so it ticks down regardless of state)
             if (_fireCooldown > 0)
            {
                _fireCooldown -= Time.deltaTime;
            }
        }

        protected override void MoveToTarget()
        {
            if (playerTarget == null) return;
        
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            
            pathUpdateTimer += Time.deltaTime;
            
            Vector3 runDir = (transform.position - playerTarget.position).normalized;
            
            // Too Close: Run away (Responsive)
            if (dist < idealDistance - distanceBuffer)
            {
                if (pathUpdateTimer >= 0.1f)
                {
                    Vector3 dest = transform.position + runDir * 3.0f;
                    agent.SetDestination(dest);
                    pathUpdateTimer = 0f;
                }
            }
            // Too Far: Chase to ideal distance (Standard Delay)
            else if (dist > idealDistance + distanceBuffer)
            {
                if (pathUpdateTimer >= PATH_UPDATE_DELAY)
                {
                    agent.SetDestination(playerTarget.position);
                    pathUpdateTimer = 0f;
                }
            }
            // Sweet spot: Stop
            else
            {
                if (pathUpdateTimer >= 0.2f)
                {
                    agent.ResetPath();
                    pathUpdateTimer = 0f;
                }
            }
            
            // SHOOT LOGIC
            if (_fireCooldown <= 0 && dist <= attackRange)
            {
                Vector3 shootDir = (playerTarget.position - transform.position).normalized;
                Shoot(shootDir);
                _fireCooldown = fireRate;
            }
        }
        
        private void Shoot(Vector3 direction)
        {
            if (projectilePrefab == null)
            {
                print("No projectile prefab assigned for ranger: " + gameObject.name);
                return;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            
            GameObject proj = Instantiate(projectilePrefab, transform.position, rotation);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                ep.Setup(projectileDamage, projectileSpeed);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, idealDistance);
        }
    }
}


