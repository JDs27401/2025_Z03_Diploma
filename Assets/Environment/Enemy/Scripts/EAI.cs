using System.Collections.Generic;
using System.Numerics;
using C__Classes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTarget;
    private float pathUpdateTimer = 0f;
    private const float PATH_UPDATE_DELAY = 0.5f;
    private enum State { Asleep, Aggravated}
    private State currentState = State.Asleep;
    
    //Animation stuff
    private Animator animator;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponent<Animator>();
        
        // PAMIĘTAJ: Jeśli w base.Start() masz jakieś dzielenie speed /= 50, 
        // to AI będzie bardzo wolne. Przy delcie operujemy na czystych wartościach.
    }
    void Update()
    {
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