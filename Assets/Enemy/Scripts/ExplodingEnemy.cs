using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class ExplodingEnemy : NpcBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("player"))
            {
                return;
            }
            Pacify();
            Agent.SetDestination(transform.position);
            animator.SetTrigger("AboutToExplode"); 
        }
    }
}