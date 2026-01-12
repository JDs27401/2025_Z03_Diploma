using System.Collections.Generic;
using System.Numerics;
using C__Classes;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AI : Actor
{
    enum State {Asleep, Aggravated};
    private State currentState = State.Asleep;

    private Vector2 target; //AI will go to that place indirectly
    private List<Vector2> subtargets; //Usually a points which AI has to reach to be able to go to target
    private Vector2 currentSpeed = Vector2.zero;
    
    //Animation stuff
    private Animator animator;
    
    new void Start()
    {
        base.Start();
        target = new Vector2(transform.position.x, transform.position.y);
        subtargets = PathFinding.Dumb(target);
        animator = GetComponent<Animator>();
    }
    new void Update()
    {
        subtargets = PathFinding.Dumb(target);
        CalculateSpeed();
        Move();
        
        UpdateAnimation();
    }

    void Move() //changing position
    {
        Vector3 newPos = new Vector3(transform.position.x + currentSpeed.x, transform.position.y + currentSpeed.y, 0);
        transform.position = newPos;
    }

    void UpdateAnimation()
    {
        bool isMoving = Mathf.Abs(currentSpeed.x) > 0.01f || Mathf.Abs(currentSpeed.y) > 0.01f;
        animator.SetBool("isWalking", isMoving);
        
        if (currentSpeed != Vector2.zero)
        {
            if(currentSpeed.x < 0)
                animator.SetFloat("XInput", -1); //Had to add this line because the zombie wouldn't flip on X axis lol
            else 
                animator.SetFloat("XInput", currentSpeed.x);
            animator.SetFloat("YInput", currentSpeed.y);    
        }
    }

    public void Aggrevate(Vector2 source)
    {
        print("agro");
        target = source;
        currentState = State.Aggravated;
        subtargets = PathFinding.Dumb(target);
    }

    public void Pacify()
    {
        print("sleep");
        currentState = State.Asleep;
    }
    Vector2 CalculateAngle()
    {
        Vector2 ToV2(Vector3 v) => new Vector2(v.x, v.y);
        Vector2 angle = target - ToV2(transform.position);
        if (Mathf.Pow(angle.x, 2) + Mathf.Pow(angle.y, 2) < 0.1)
        {
            return Vector2.zero;
        }

        if (angle == Vector2.zero)
        {
            return Vector2.zero;
        }
        if (Mathf.Abs(angle.x) >= Mathf.Abs(angle.y))
        {
            angle /= Mathf.Abs(angle.x);
        }
        else
        {
            angle /= Mathf.Abs(angle.y);
        }
        return angle;
    }
    void CalculateSpeed()
    {
        //calculatoration
        currentSpeed *= friction;
        
        currentSpeed += CalculateAngle()*acceleration;
        
        //check for max speed
        if (currentSpeed.x >= speed)
        {
            currentSpeed.x = speed;
        }

        if (currentSpeed.x <= -speed)
        {
            currentSpeed.x = -speed;
        }
        if (currentSpeed.y >= speed)
        {
            currentSpeed.y = speed;
        }
        if (currentSpeed.y <= -speed)
        {
            currentSpeed.y = -speed;
        }
    }

    // void OnTriggerStay2D(Collider2D collision)
    // {
    //     if (collision.gameObject.tag == "player")
    //     {
    //         collision.gameObject.GetComponent<Actor>().DealDamage(10);
    //         collision.gameObject.GetComponent<Actor>().StartInvulnerability();
    //     }
    // }
}
