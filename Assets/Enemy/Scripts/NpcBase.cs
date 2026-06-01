using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using C__Classes;
using C__Classes.Commands;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
        
        //for animation timing (so it doesn't loop)
        private float lastHurtTime = -1f;
        [SerializeField] private float hurtAnimCooldown = 0.5f; 

        // private Actor actor;
        
        //Animation stuff
        protected Animator animator;
        
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
            
            // PAMIĘTAJ: Jeśli w base.Start() masz jakieś dzielenie speed /= 50, 
            // to AI będzie bardzo wolne. Przy delcie operujemy na czystych wartościach.
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
            if (!(Mathf.Abs(currentHealth - _lastKnownHealth) > 0.01f))
            {
                return;
            }
            
            //We check if the health is lower than last known health so we can play hurt or death animation
            if (!(currentHealth < _lastKnownHealth))
            {
                return;
            }
            
            if (currentHealth <= 0)
            {
                animator.SetTrigger("Die");
            }
            else
            {
                //Added so the animation doesn't loop
                if (!(Time.time >= lastHurtTime + hurtAnimCooldown))
                {
                    return;
                }
                
                animator.SetTrigger("Hurt");
                lastHurtTime = Time.time;
            }
            
            _lastKnownHealth = currentHealth;
        }
        
        // void MoveToTarget()
        // {
        //     if (!playerTarget) return;
        //
        //     pathUpdateTimer += Time.deltaTime;
        //     if (pathUpdateTimer >= PATH_UPDATE_DELAY)
        //     {
        //         Agent.SetDestination(playerTarget.position);
        //         pathUpdateTimer = 0f;
        //     }
        // }

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
        }

        public void Pacify()
        {
            PlayerTarget = null;
            currentState = State.Asleep;
        }

        public override void SetSpeed(float s)
        {
            base.SetSpeed(s);
            Agent.speed = speed;
        }
    }
}