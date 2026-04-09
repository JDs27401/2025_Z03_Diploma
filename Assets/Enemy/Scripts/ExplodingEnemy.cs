using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class ExplodingEnemy : NpcBase
    {
        protected override void Start()
        {
            base.Start();
            Aggravate(GameObject.FindWithTag("player").transform);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("player"))
            {
                return;
            }
            Pacify();
            agent.SetDestination(transform.position);
            animator.SetTrigger("AboutToExplode"); 
        }
    }
}