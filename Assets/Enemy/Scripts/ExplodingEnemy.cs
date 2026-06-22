using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class ExplodingEnemy : NpcBase
    {
        
        //Audio
        public event Action OnExplode;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("player"))
            {
                return;
            }
            Agent.isStopped = true;
            Agent.velocity = Vector3.zero;
            animator.SetTrigger("AboutToExplode"); 
            OnExplode?.Invoke();
        }
    }
}