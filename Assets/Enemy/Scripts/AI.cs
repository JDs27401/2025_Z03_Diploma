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
        
        // PAMIĘTAJ: Jeśli w base.Start() masz jakieś dzielenie speed /= 50, 
        // to AI będzie bardzo wolne. Przy delcie operujemy na czystych wartościach.
    }
    new void Update()
    {
        // Tutaj używamy zwykłego Update, więc delta będzie zmienna (zależna od FPS)
        subtargets = PathFinding.Dumb(target);
        CalculateSpeed();
        Move();
        
        UpdateAnimation();
    }

    void Move() //changing position
    {
        // DODANO DELTĘ: Przesunięcie = Prędkość * Czas (Time.deltaTime)
        Vector3 displacement = new Vector3(currentSpeed.x, currentSpeed.y, 0) * Time.deltaTime;
        transform.position += displacement;
    }

    void UpdateAnimation()
    {
        bool isMoving = Mathf.Abs(currentSpeed.x) > 0.01f || Mathf.Abs(currentSpeed.y) > 0.01f;
        animator.SetBool("isWalking", isMoving);
        
        if (currentSpeed != Vector2.zero)
        {
            if(currentSpeed.x < 0)
                animator.SetFloat("XInput", -1); 
            else 
                animator.SetFloat("XInput", currentSpeed.x); // Tu warto by dać Mathf.Sign lub 1, żeby animacja się nie psuła przy małych prędkościach
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
        
        // DODANO DELTĘ: Akceleracja musi uwzględniać czas klatki
        // V = a * t. Używamy Time.deltaTime, bo jesteśmy w Update()
        currentSpeed += CalculateAngle() * acceleration * Time.deltaTime;
        
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
}