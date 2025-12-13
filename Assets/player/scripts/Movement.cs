using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : Debuggable
{
    [SerializeField] //max speed stat
    private float speed = 4f;
    [SerializeField]
    private int maxStamina = 100;
    [SerializeField] //how fast will player gain speed
    private float acceleration = 2f;
    [SerializeField] //how fast will the player slow down (the higher the more friction)
    private float friction = 0.10f;
    [SerializeField] //true if you want to know all the stuff
    private bool debugInfo = true;
    [SerializeField] //duration between rolls
    private float rollPower = 10;
    [SerializeField] //duration between rolls
    private float rollCoodownSeconds = 3;
    [SerializeField] //duration of a single roll
    private float rollDurationSeconds = 1;
    
    private Vector2 moveInput; 
    private Vector2 currentSpeed = Vector2.zero;
    private bool isRolling = false;
    private bool rollCooldown = false;
    private long lastRollTime = 0;
    private bool isCrouching = false;
    private SpriteRenderer spriteRenderer;
    private float playerColorAlpha = 1;
    private bool isSprinting = false;
    private int stamina = 100;
    
    void Start(){
        speed /= 50; //To have nicer vales
        acceleration /= 10;
        friction = 1-friction;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        ManageSprint();
        ManageCrouch();
        ManageRoll();
        
        if (!isRolling)
        {
            CalculateSpeed();
        }
        
        Move();

        if (debugInfo)
        {
            PrintVariables("currentSpeed", "moveInput", "transform.position", "isRolling", "rollCooldown", "stamina");
        }
    }
    
    
    public void WSADManagement(InputAction.CallbackContext context){ //getting directions from keys pressed
        moveInput = context.ReadValue<Vector2>();
    }
    public void SpaceManagement(InputAction.CallbackContext context)
    {
        if (!rollCooldown && !isRolling)
        {
            stamina -= 20;
            isRolling = true;
            rollCooldown = true;
            lastRollTime = DateTime.Now.Ticks;
            currentSpeed *= rollPower;
        }
    }
    public void CtrlManagement(InputAction.CallbackContext context)
    {
        isCrouching = !isCrouching;
    }
    public void ShiftManagement(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && stamina > 20)
        {
            isSprinting = true;
            speed *= 3;
        }

        if (context.phase == InputActionPhase.Canceled && isSprinting)
        {
            isSprinting = false;
            speed /= 3;
        }
    }
    
    
    void CalculateSpeed()
    {
        //calculatoration
        currentSpeed *= friction;
        currentSpeed += moveInput*acceleration;
        
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
    void Move() //changing player position
    {
        Vector3 newPos = new Vector3(transform.position.x + currentSpeed.x, transform.position.y + currentSpeed.y, 0);
        transform.position = newPos;
    }
    void ManageRoll()
    {
        if (DateTime.Now.Ticks - lastRollTime >= rollCoodownSeconds * 10000000)
        {
            rollCooldown = false;
        }
        if (DateTime.Now.Ticks - lastRollTime >= rollDurationSeconds * 10000000)
        {
            isRolling = false;
        }

        if (isRolling)
        {
            currentSpeed *= 0.8f;
        }
    }
    void ManageCrouch()
    {
        if (isCrouching)
        {
            if(playerColorAlpha > 0.7) playerColorAlpha -= 0.01f;
        }
        else
        {
            if(playerColorAlpha < 1) playerColorAlpha += 0.01f;
        }
        var newColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, playerColorAlpha);
        spriteRenderer.color = newColor;
    }
    void ManageSprint()
    {
        print("Stamina: (" + stamina + "/" + maxStamina + ")");
        if (isSprinting)
        {
            if (stamina <= 0)
            {
                isSprinting = false;
                speed /= 3;
            }

            stamina -= 1;
        }
        else
        {
            if (stamina < maxStamina)
            {
                stamina += 1;
            }
        }
    }
}
