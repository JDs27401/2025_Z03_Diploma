using Enemy.Scripts;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace C__Classes.Commands
{
    public class IdleCommand : Command
    {
        private NavMeshAgent _agent;
        private float _pathUpdateTimer;
        private float _pathUpdateDelay;
        
        public IdleCommand(NpcBase npc, float pathUpdateDelay) : base(npc)
        {
            _agent = npc.Agent;
            _pathUpdateTimer = npc.PathUpdateTimer;
            _pathUpdateDelay = pathUpdateDelay;
            // _pathUpdateDelay = npc.PathUpdateDelay;
        }
        
        public override void Execute()
        {
            System.Random r = new System.Random();
            Vector3 destination = new Vector3(npc.transform.position.x - r.Next(-2, 2), npc.transform.position.y - r.Next(-2, 2), 0);

            _pathUpdateTimer += Time.deltaTime;
            if (_pathUpdateTimer < _pathUpdateDelay)
            {
                return;
            }
            _agent.SetDestination(destination);
            _pathUpdateTimer = 0f;
        }
    }
}