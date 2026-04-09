using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class BlindEnemy : NpcBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            #if UNITY_EDITOR
            print("Entered Noise Component");
            #endif
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            
            Aggravate(FindFirstObjectByType<PlayerController>().transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            #if UNITY_EDITOR
            print("Left Noise Component");
            #endif
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            
            Pacify();
            agent.SetDestination(transform.position);
        }
    }
}