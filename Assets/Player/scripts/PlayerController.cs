using System;
using C__Classes;
using C__Classes.Managers;
using C__Classes.Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : Actor
{
    // Lukasz, health bar event
    [Header("UI Events")]
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onStaminaChanged;
    
    private float _lastKnownHealth;
    private int _lastKnownStamina;
    
    [Header("Keys management")]
    [SerializeField] 
    private InputActionReference moveAction;
    [SerializeField] 
    private InputActionReference sprintAction;
    
    [Header("Movement stats")]
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
    
    [Header("Weight & Equipment")]
    [SerializeField] private float speedPenaltyPerPoint = 0.01f; // Speed penalty per 1 weight point (ex. 0.01 = 1% loss for 1 kg)
    [SerializeField] private float minimumSpeedPercentage = 0.15f; // 15% of base movement speed
    
    private PlayerInput playerInput;
    private Vector2 moveInput; 
    private Vector2 currentSpeed = Vector2.zero;
    private bool isRolling = false;
    private bool rollCooldown = false;
    private float lastRollTime = 0f;
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
    
    
    protected override void Start(){
        base.Start();
        
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            var playerMap = playerInput.actions.FindActionMap("Player");
            if (playerMap != null)
            {
                var action = playerMap.FindAction("Jump");
                if (action != null) action.performed += SpaceManagement; else {
                    Debug.LogWarning("Jump action not found in Player action map.");
                }
                var findAction = playerMap.FindAction("Crouch");
                if (findAction != null) findAction.performed += CtrlManagement;
            }else{
                Debug.LogWarning("Player action map not found in PlayerInput.");
            }
        }else{
            Debug.LogWarning("PlayerInput component not found on PlayerController.");
        }
        
        mainCam = Camera.main;
        if (mainCam == null)
        {
            mainCam = FindFirstObjectByType<Camera>();    
        }
        
        friction = 1-friction;
        spriteRenderer = GetComponent<SpriteRenderer>();

        _lastKnownHealth = currentHealth;
        float healthPercent = currentHealth / maxHealth;
        onHealthChanged?.Invoke(healthPercent);

        _lastKnownStamina = stamina;
        float staminaPercent = (maxStamina > 0) ? (float)stamina / maxStamina : 0;
        onStaminaChanged?.Invoke(staminaPercent);
    }

    new void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        UpdateSprintInput();
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

    private void UpdateSprintInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        var sprintHeld = sprintAction?.action != null && sprintAction.action.IsPressed();

        switch (sprintHeld)
        {
            case true when !isSprinting && stamina > 20:
                isSprinting = true;
                speed *= sprintPower;
                acceleration *= sprintPower;
                break;
            case false when isSprinting:
                isSprinting = false;
                speed /= sprintPower;
                acceleration /= sprintPower;
                break;
        }

        PlayerNoiseSystem.Instance.UpdateNoiseRadius();
    }
    public void SpaceManagement(InputAction.CallbackContext context)
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
        if (rollCooldown || isRolling) return;
        stamina -= rollStaminaCost;
        isRolling = true;
        rollCooldown = true;
        lastRollTime = Time.time;
        currentSpeed *= rollPower;
    }
    public void CtrlManagement(InputAction.CallbackContext context)
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
        isCrouching = !isCrouching;
        PlayerVisualSystem.Instance.UpdateVisualRange(isCrouching);
    }
    
    void CalculateSpeed()
    {
        float weightPenaltyMultiplier = 1f;

        if (InventoryManager.Instance != null)
        {
            float currentWeight = InventoryManager.Instance.GetTotalWeight();
            
            weightPenaltyMultiplier = Mathf.Max(minimumSpeedPercentage, 1f - (currentWeight * speedPenaltyPerPoint));
        }

        float currentEffectiveAcceleration = acceleration * weightPenaltyMultiplier;
        float currentEffectiveSpeed = speed * weightPenaltyMultiplier;

        currentSpeed *= friction;
        currentSpeed += moveInput * (currentEffectiveAcceleration * Time.fixedDeltaTime); 
        
        if (currentSpeed.x >= currentEffectiveSpeed)
        {
            currentSpeed.x = currentEffectiveSpeed;
        }
        if (currentSpeed.x <= -currentEffectiveSpeed)
        {
            currentSpeed.x = -currentEffectiveSpeed;
        }
        if (currentSpeed.y >= currentEffectiveSpeed)
        {
            currentSpeed.y = currentEffectiveSpeed;
        }
        if (currentSpeed.y <= -currentEffectiveSpeed)
        {
            currentSpeed.y = -currentEffectiveSpeed;
        }
    }

    void Move() 
    {
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
        
        Vector3 mouseScreenPos = (Vector3)Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);

        Vector2 direction = (mainCam.ScreenToWorldPoint(mouseScreenPos) - transform.position).normalized;
        
        animator.SetFloat("XInput", direction.x);
        animator.SetFloat("YInput", direction.y);
    }

    public void SetWeaponAnimation(int weaponID)
    {
        if (animator != null)
        {
            animator.SetInteger("WeaponID", weaponID);
        }
    }
    
    void ManageRoll()
    {
        if (Time.time - lastRollTime >= rollCoodownSeconds)
        {
            rollCooldown = false;
        }
        if (Time.time - lastRollTime >= rollDurationSeconds)
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
    
    public void ClearInputState()
    {
        moveInput = Vector2.zero;
        currentSpeed = Vector2.zero;
        isSprinting = false;
    }

    public bool IsCrouching() => isCrouching;
    public bool IsSprinting() => isSprinting;
}