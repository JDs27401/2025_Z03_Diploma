using System.Collections.Generic;
using System.Numerics;
using C__Classes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EAI : Actor
{
    private NavMeshAgent agent;
    private Transform playerTarget;
    private float pathUpdateTimer = 0f;
    private const float PATH_UPDATE_DELAY = 0.5f;
    private enum State { Asleep, Aggravated}
    private State currentState = State.Asleep;
    
    private float _lastKnownHealth;
    
    //for animation timing (so it doesn't loop)
    private float lastHurtTime = -1f;
    [SerializeField] private float hurtAnimCooldown = 0.5f; 

    private Actor actor;
    
    //Animation stuff
    private Animator animator;
    
    new void Start()
    {
        base.Start();
        
        actor = GetComponent<Actor>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponent<Animator>();
        
        _lastKnownHealth = currentHealth;
        
        // PAMIĘTAJ: Jeśli w base.Start() masz jakieś dzielenie speed /= 50, 
        // to AI będzie bardzo wolne. Przy delcie operujemy na czystych wartościach.
    }
    new void Update()
    {
        base.Update();
        
        if (Mathf.Abs(currentHealth - _lastKnownHealth) > 0.01f)
        {
            //We check if the health is lower than last known health so we can play hurt or death animation
            if (currentHealth < _lastKnownHealth)
            {
                if (currentHealth <= 0)
                {
                    animator.SetTrigger("Die");
                }
                else
                {
                    //Added so the animation doesn't loop
                    if (Time.time >= lastHurtTime + hurtAnimCooldown)
                    {
                        animator.SetTrigger("Hurt");
                        lastHurtTime = Time.time;
                    }
                }
            }
            
            
            _lastKnownHealth = currentHealth;
        }
        
        if (isDead)
        {
            return;
        }
        //
        switch (currentState)
        {
            case State.Aggravated:
                MoveToTarget();
                break;
            case State.Asleep:
                // [W] opcjonalny patrol?
                break;
        }
        FixZPosition();
        UpdateAnimation();
    }
    
    void MoveToTarget()
    {
        if (!playerTarget) return;

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= PATH_UPDATE_DELAY)
        {
            agent.SetDestination(playerTarget.position);
            pathUpdateTimer = 0f;
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
        bool isMoving = Mathf.Abs(agent.velocity.x) > 0.01f || Mathf.Abs(agent.velocity.y) > 0.01f;
        animator.SetBool("isWalking", isMoving);
        
        if (V3toV2(agent.velocity) != Vector2.zero)
        {
            if(agent.velocity.x < 0)
                animator.SetFloat("XInput", -1); 
            else 
                animator.SetFloat("XInput", agent.velocity.x); // Tu warto by dać Mathf.Sign lub 1, żeby animacja się nie psuła przy małych prędkościach
            animator.SetFloat("YInput", agent.velocity.y);    
        }
    }

    private Vector2 V3toV2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public void Aggravate(Transform target)
    {
        playerTarget = target;
        currentState = State.Aggravated;
        pathUpdateTimer = PATH_UPDATE_DELAY;
    }

    public void Pacify()
    {
        playerTarget = null;
        currentState = State.Asleep;
    }
}