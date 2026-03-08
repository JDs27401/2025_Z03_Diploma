using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class BlindEnemy : EAI
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            print("Entered Noise Component");
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            
            Aggravate(FindFirstObjectByType<PlayerController>().transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            print("Left Noise Component");
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            
            Pacify();
        }
    }
}