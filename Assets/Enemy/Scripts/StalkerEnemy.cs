using System.Collections.Generic;
using UnityEngine;
using Enemy.Scripts;

namespace Enemy.Scripts
{
    public class StalkerEnemy : NpcBase
    {
        [Header("Stalker Settings")]
        [SerializeField] private float warningRadius = 15f;
        [SerializeField] private float alertFrequency = 1.0f;
        [SerializeField] private float stalkingSpeed = 2f;
        
        private float _alertTimer = 0f;

        protected override void Start()
        {
            base.Start();
            
            damage = 0f; 
            agent.speed = stalkingSpeed; 
        }

        protected override void Update()
        {
            base.Update();
            
            // Player is in FOV
            if (currentState == State.Aggravated && playerTarget != null)
            {
                HandleAlertPulse();
            }
        }

        private void HandleAlertPulse()
        {
            _alertTimer += Time.deltaTime;
            if (_alertTimer >= alertFrequency)
            {
                _alertTimer = 0f;
                AlertNearbyZombies();
            }
        }

        private void AlertNearbyZombies()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, warningRadius);
            
            foreach (var hit in hits)
            {
                // Don't alert self
                if (hit.gameObject == gameObject) continue;

                // Check if it's an NpcBase
                NpcBase npc = hit.GetComponent<NpcBase>();
                if (npc != null)
                {
                    // Should we re-alert other Stalkers? (possible chain reaction loop) 
                    // For now we simply pass the player target.
                    npc.Aggravate(playerTarget);
                }
            }
        }
        
        // Debug visualization for warning radius
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, warningRadius);
        }
    }
}


