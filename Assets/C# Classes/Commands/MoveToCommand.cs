using Enemy.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace C__Classes.Commands
{
    public class MoveToCommand : Command
    {
        private NavMeshAgent _agent;
        private Transform _target;
        private float _pathUpdateTimer;
        private float _pathUpdateDelay;

        public MoveToCommand(NpcBase npc) : base(npc)
        {
            _agent = npc.Agent;
            _target = npc.PlayerTarget;
            _pathUpdateTimer = npc.PathUpdateTimer;
            _pathUpdateDelay = npc.PathUpdateDelay;
        }
        
        public override void Execute()
        {
            if (!_target) return;

            _pathUpdateTimer += Time.deltaTime;
            if (_pathUpdateTimer < _pathUpdateDelay)
            {
                return;
            }
            _agent.SetDestination(_target.position);
            _pathUpdateTimer = 0f;
        }
    }
}