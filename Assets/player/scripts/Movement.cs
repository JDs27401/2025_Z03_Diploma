using System;
using C__Classes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Movement : Actor
{
    // Lukasz, health bar event
    [Header("UI Events")]
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onStaminaChanged;
    
    private float _lastKnownHealth;
    private int _lastKnownStamina;

    //Wiktor
    [SerializeField]
    private int maxStamina = 100;
    [SerializeField] //true if you want to know all the stuff
    private bool debugInfo = true;
    [SerializeField]
    private float rollPower = 40;
    [SerializeField]
    private int rollStaminaCost = 20;
    [SerializeField] //duration between rolls
    private float rollCoodownSeconds = 3;
    [SerializeField] //duration of a single roll
    private float rollDurationSeconds = 0.5f;
    [SerializeField] 
    private float sprintPower = 2; //multiplying speed by this while sprinting 
    
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
    
    //Thing I need for animations - Bartek
    [SerializeField]
    private Animator animator;
    private Camera mainCam;
    private Vector3 mousePos;
    
    
    new void Start(){
        base.Start();
        mainCam = Camera.main;
        if (mainCam == null)
        {
            mainCam = FindFirstObjectByType<Camera>();    
        }
        // speed /= 50; //To have nicer vales
        // acceleration /= 10;
        friction = 1-friction;
        spriteRenderer = GetComponent<SpriteRenderer>();

        _lastKnownHealth = currentHealth;
        float healthPercent = currentHealth / maxHealth;
        onHealthChanged?.Invoke(healthPercent);

        _lastKnownStamina = stamina;
        float staminaPercent = (maxStamina > 0) ? (float)stamina / maxStamina : 0;
        onStaminaChanged?.Invoke(staminaPercent);
    }
    
    void FixedUpdate()
    {
        CheckForHurtAnimation();
        ManageStamina();
        if (base.isDead)
        {
            return;
        }
        ManageSprint();
        ManageCrouch();
        ManageRoll();
        if (!isRolling)
        {
            CalculateSpeed();
        }
        Move();
        UpdateAnimations();
    }

    private void ManageStamina()
    {
        if (stamina != _lastKnownStamina)
        {
            _lastKnownStamina = stamina;
            
            float staminaPercent = (maxStamina > 0) ? (float)stamina / maxStamina : 0;
            
            onStaminaChanged?.Invoke(staminaPercent);
        }
    }

    private void CheckForHurtAnimation()
    {
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
                    animator.SetTrigger("Hurt");
                }
            }
            
            
            _lastKnownHealth = currentHealth;
        
            float healthPercent = (maxHealth > 0) ? currentHealth / maxHealth : 0;
            
            onHealthChanged?.Invoke(healthPercent);
        }
    }
    
    public void WSADManagement(InputAction.CallbackContext context){ //getting directions from keys pressed
        moveInput = context.ReadValue<Vector2>();
    }
    public void SpaceManagement(InputAction.CallbackContext context)
    {
        if (!rollCooldown && !isRolling)
        {
            stamina -= rollStaminaCost;
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
            speed *= sprintPower;
            acceleration *= sprintPower;
        }

        if (context.phase == InputActionPhase.Canceled && isSprinting)
        {
            isSprinting = false;
            speed /= sprintPower;
            acceleration /= sprintPower;
        }
    }
    
    
    void CalculateSpeed()
    {
        //calculatoration
        currentSpeed *= friction;
        currentSpeed += moveInput * acceleration * Time.fixedDeltaTime; //Added delta
        
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
        print(currentSpeed);
        print("a: " + acceleration);
    }
    void Move() //changing player position
    {
        //Added delta
        Vector3 newPos = new Vector3(
            transform.position.x + currentSpeed.x * Time.fixedDeltaTime,
            transform.position.y + currentSpeed.y * Time.fixedDeltaTime,
            0);
        transform.position = newPos;
    }

    void UpdateAnimations()
    {
        if (mainCam == null || Mouse.current == null) return;
        
        
        bool isMoving = Mathf.Abs(currentSpeed.x) > 1f || Mathf.Abs(currentSpeed.y) > 1f;
        animator.SetBool("isWalking", isMoving);
        
        //Calculating where player is looking based on the mouse position
        Vector3 mouseScreenPos = (Vector3)Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);

        Vector2 direction = (mainCam.ScreenToWorldPoint(mouseScreenPos) - transform.position).normalized;

        
        animator.SetFloat("XInput", direction.x);
        animator.SetFloat("YInput", direction.y);
        
        
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
            if(playerColorAlpha > 0.4) playerColorAlpha -= 0.05f;
        }
        else
        {
            if(playerColorAlpha < 1) playerColorAlpha += 0.05f;
        }
        var newColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, playerColorAlpha);
        spriteRenderer.color = newColor;
    }
    void ManageSprint()
    {
        if (isSprinting)
        {
            if (stamina <= 0)
            {
                isSprinting = false;
                speed /= sprintPower;
                acceleration /= sprintPower;
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

    public bool IsCrouching() => isCrouching;
}