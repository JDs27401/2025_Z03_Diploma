using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using C__Classes;
using C__Classes.Commands;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Enemy.Scripts
{
    public class NpcBase : Actor
    {
        public NavMeshAgent Agent { get; private set; }
        public Transform PlayerTarget { get; set; }
        public float PathUpdateTimer { get; set; } = 0f;
        public float PathUpdateDelay { get; private set; } = 0.5f;
        public enum State { Asleep, Aggravated}
        protected State currentState = State.Asleep;
        
        private float _lastKnownHealth;
        
        //Animation stuff
        protected Animator animator;
        
        //Audio events
        public event Action OnHurt;
        public event Action OnDeath;
        public event Action OnAggravated;
        public event Action OnSleep;
        
        [SerializeField] private float agentRadius = 0.05f;
        
        private Command _command;
        
        protected override void Start()
        {
            base.Start();
            
            // actor = GetComponent<Actor>();
            Agent = GetComponent<NavMeshAgent>();
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;
            Agent.speed = baseSpeed;
            
            Agent.radius = agentRadius;
            Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            Agent.avoidancePriority = Random.Range(0, 100);
            
            animator = GetComponent<Animator>();
            
            _lastKnownHealth = currentHealth;
        }
        protected override void Update()
        {
            base.Update();

            CheckForHurtAnimation();
            
            switch (currentState)
            {
                case State.Aggravated:
                    if (_command == null || _command.GetType() != typeof(MoveToCommand))
                    {
                        _command = new MoveToCommand(this);
                    }
                    else
                    {
                        _command.Execute();
                    }
                    break;
                case State.Asleep:
                    if (_command == null || _command.GetType() != typeof(IdleCommand))
                    {
                        _command = new IdleCommand(this, 2);
                    }
                    else
                    {
                        _command.Execute();
                    }
                    break;
            }
            FixZPosition();
            UpdateAnimation();
        }

        private void CheckForHurtAnimation()
        {
            if (Mathf.Abs(currentHealth - _lastKnownHealth) < 0.01f)
            {
                return;
            }
            
            bool tookDamage = currentHealth < _lastKnownHealth;
            
            _lastKnownHealth = currentHealth;
            
            if(!tookDamage)
                return;
            
            if (currentHealth <= 0)
            {
                HandleDeath();
                animator.SetTrigger("Die");
                OnDeath?.Invoke();
            }
            else
            {
               animator.Play("Hurt Blend Tree", -1, 0f);
                OnHurt?.Invoke();
            }
            
            _lastKnownHealth = currentHealth;
        }

        private void HandleDeath()
        {
            if (Agent is not null)
            {
                Agent.isStopped = true;
            }
        }

        void FixZPosition()
        {
            if (Mathf.Abs(transform.position.z) > 0.01f)
            {
                Vector3 pos = transform.position;
                pos.z = 0;
                transform.position = pos;
            }
        }
        
        

        void UpdateAnimation()
        {
            bool isMoving = Mathf.Abs(Agent.velocity.x) > 0.01f || Mathf.Abs(Agent.velocity.y) > 0.01f;
            animator.SetBool("isWalking", isMoving);
            
            if (V3toV2(Agent.velocity) != Vector2.zero)
            {
                if(Agent.velocity.x < 0)
                    animator.SetFloat("XInput", -1); 
                else 
                    animator.SetFloat("XInput", Agent.velocity.x); // Tu warto by dać Mathf.Sign lub 1, żeby animacja się nie psuła przy małych prędkościach
                animator.SetFloat("YInput", Agent.velocity.y);    
            }
        }

        private Vector2 V3toV2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public void Aggravate(Transform target)
        {
            PlayerTarget = target;
            currentState = State.Aggravated;
            PathUpdateTimer = PathUpdateDelay;
            
            OnAggravated?.Invoke();
        }

        public void Pacify()
        {
            PlayerTarget = null;
            currentState = State.Asleep;
            OnSleep?.Invoke();
        }

        public override void SetSpeed(float s)
        {
            base.SetSpeed(s);
            Agent.speed = speed;
        }
    }
}