using System;
using C__Classes;
using C__Classes.Managers;
using C__Classes.Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : Actor
{
    public UnityEvent onPlayerDeath;
    
    // Lukasz, health bar event
    [Header("UI Events")]
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onStaminaChanged;
    
    private float _lastKnownHealth;
    private int _lastKnownStamina;
    
    [Header("Keys management")]
    private PlayerInput playerInput;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction useConsumableAction;
    
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
    
    
            private void Awake()
            {
                playerInput = GetComponent<PlayerInput>();
                CacheInputActions();
            }

    protected override void Start(){
        base.Start();
                if (playerInput != null && playerActionMap != null)
                {
                    var action = playerActionMap.FindAction("Jump");
                    if (action != null) action.performed += SpaceManagement; else {
                        Debug.LogWarning("Jump action not found in Player action map.");
                    }
                    var findAction = playerActionMap.FindAction("Crouch");
                    if (findAction != null) findAction.performed += CtrlManagement;

                    useConsumableAction = playerActionMap.FindAction("UseConsumable");
                    if (useConsumableAction != null) useConsumableAction.performed += UseConsumableManagement;
                }
                else if (playerInput == null)
                {
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
        float healthPercent = GetCurrentHealthPercent();
        onHealthChanged?.Invoke(healthPercent);

        _lastKnownStamina = stamina;
        float staminaPercent = (maxStamina > 0) ? (float)stamina / maxStamina : 0;
        onStaminaChanged?.Invoke(staminaPercent);
    }

    new void Update()
    {
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        UpdateSprintInput();
    }
    
    void FixedUpdate()
    {
        if (base.isDead)
        {
            return;
        }

        ApplyConsumableEffects();
        CheckForHurtAnimation();
        ManageStamina();
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
            float healthPercent = GetCurrentHealthPercent();
            onHealthChanged?.Invoke(healthPercent);
        }
    }

    private void UpdateSprintInput()
    {
        if (sprintAction == null)
        {
            return;
        }

        var sprintHeld = sprintAction.IsPressed();

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

    private void CacheInputActions()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogWarning("PlayerInput albo InputActionAsset nie są ustawione na PlayerController.");
            return;
        }

        playerActionMap = playerInput.actions.FindActionMap("Player", false);
        if (playerActionMap == null)
        {
            Debug.LogWarning("Player action map not found in PlayerInput.");
            return;
        }

        moveAction = playerActionMap.FindAction("Move", false);
        sprintAction = playerActionMap.FindAction("Sprint", false);

        if (moveAction == null)
        {
            Debug.LogWarning("Move action not found in Player action map.");
        }

        if (sprintAction == null)
        {
            Debug.LogWarning("Sprint action not found in Player action map.");
        }
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

    public void UseConsumableManagement(InputAction.CallbackContext context)
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
        if (InventoryManager.Instance == null) return;

        InventoryManager.Instance.TryUseActiveConsumable();
    }
    
    void CalculateSpeed()
    {
        float weightPenaltyMultiplier = 1f;
        float consumableSpeedMultiplier = GetConsumableSpeedMultiplier();
        float consumableAccelerationMultiplier = GetConsumableAccelerationMultiplier();

        if (InventoryManager.Instance != null)
        {
            float currentWeight = InventoryManager.Instance.GetTotalWeight();
            
            weightPenaltyMultiplier = Mathf.Max(minimumSpeedPercentage, 1f - (currentWeight * speedPenaltyPerPoint));
        }

        float currentEffectiveAcceleration = acceleration * weightPenaltyMultiplier * consumableAccelerationMultiplier;
        float currentEffectiveSpeed = speed * weightPenaltyMultiplier * consumableSpeedMultiplier;

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

    private void ApplyConsumableEffects()
    {
        float effectiveMaxHealth = GetEffectiveMaxHealth();
        if (currentHealth > effectiveMaxHealth)
        {
            currentHealth = effectiveMaxHealth;
        }

        float healthPerSecond = GetConsumableHealthPerSecond();
        if (healthPerSecond > 0f)
        {
            currentHealth = Mathf.Min(currentHealth + (healthPerSecond * Time.fixedDeltaTime), effectiveMaxHealth);
        }
    }

    private float GetCurrentHealthPercent()
    {
        float effectiveMaxHealth = Mathf.Max(0.01f, GetEffectiveMaxHealth());
        return currentHealth / effectiveMaxHealth;
    }

    private float GetEffectiveMaxHealth()
    {
        float bonus = 0f;
        if (InventoryManager.Instance != null)
        {
            bonus = InventoryManager.Instance.GetConsumableMaxHealthBonus();
        }

        return Mathf.Max(1f, maxHealth + bonus);
    }

    private float GetConsumableSpeedMultiplier()
    {
        if (InventoryManager.Instance == null)
        {
            return 1f;
        }

        return Mathf.Max(0f, InventoryManager.Instance.GetConsumableSpeedMultiplier());
    }

    private float GetConsumableAccelerationMultiplier()
    {
        if (InventoryManager.Instance == null)
        {
            return 1f;
        }

        return Mathf.Max(0f, InventoryManager.Instance.GetConsumableAccelerationMultiplier());
    }

    private float GetConsumableHealthPerSecond()
    {
        if (InventoryManager.Instance == null)
        {
            return 0f;
        }

        return Mathf.Max(0f, InventoryManager.Instance.GetConsumableHealthPerSecond());
    }

    private void OnDestroy()
    {
        if (playerActionMap == null)
        {
            return;
        }

        var action = playerActionMap.FindAction("Jump");
        if (action != null) action.performed -= SpaceManagement;

        var crouchAction = playerActionMap.FindAction("Crouch");
        if (crouchAction != null) crouchAction.performed -= CtrlManagement;

        if (useConsumableAction != null)
        {
            useConsumableAction.performed -= UseConsumableManagement;
        }
    }

    protected override void Kill()
    {
        base.Kill();
        onPlayerDeath?.Invoke();
    }

    public bool IsCrouching() => isCrouching;
    public bool IsSprinting() => isSprinting;
}