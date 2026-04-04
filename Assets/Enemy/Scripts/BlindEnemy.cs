using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class BlindEnemy : NpcBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            #if UNITY_EDITOR
            print("Entered Noise Component");
            #endif
            Aggravate(FindFirstObjectByType<PlayerController>().transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            #if UNITY_EDITOR
            print("Left Noise Component");
            #endif
            
            Pacify();
            agent.SetDestination(transform.position);
        }
    }
}