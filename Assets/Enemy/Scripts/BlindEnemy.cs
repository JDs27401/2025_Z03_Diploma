using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Scripts;

namespace Enemy.Scripts
{
    public class BlindEnemy : NpcBase
    {
        [Header("Blind Enemy Stats")]
        [SerializeField] private float blindSpeed = 6f;
        [SerializeField] private float blindDamage = 20f;
        
        [Header("Blind Behavior")]
        [SerializeField] private float aggroTimeout = 2.0f;
        private float _timeSinceLastAggro = 0f;

        protected override void Start()
        {
            speed = blindSpeed;
            damage = blindDamage;
            
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            
            if (currentState == State.Aggravated)
            {
                _timeSinceLastAggro += Time.deltaTime;
                if (_timeSinceLastAggro > aggroTimeout)
                {
                    Pacify();
                }
            }
        }
        
        public override void Aggravate(Transform target)
        {
            base.Aggravate(target);
            _timeSinceLastAggro = 0f;
        }
    }
}