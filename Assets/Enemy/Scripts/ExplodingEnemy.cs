using System;
using UnityEngine;

namespace Enemy.Scripts
{
    public class ExplodingEnemy : NpcBase
    {
        protected override void Start()
        {
            base.Start();
            // Aggravate(GameObject.FindWithTag("player").transform);
            playerTarget = GameObject.FindWithTag("player").transform;
            currentState = State.Aggravated;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("player"))
            {
                return;
            }
            Pacify();
            agent.SetDestination(transform.position);
            
            // animator.SetTrigger("AboutToExplose"); //zagadać do bartka apropo bo może uda się przenieść start animacji tutaj
        }
    }
}