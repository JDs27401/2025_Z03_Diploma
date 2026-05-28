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
    private float _lastKnownStamina;
    
    [Header("Keys management")]
    private PlayerInput playerInput;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction useConsumableAction;
    
    [Header("Movement stats")]
    [SerializeField]
    private int maxStamina = 100;
    // Internal float mirror for fractional stamina regen
    private float staminaFloat = 0f;
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
    
    //roll function stuff
    private bool isRolling = false;
    private bool rollCooldown = false;
    private float lastRollTime = 0f;
    
    //crouching function stuff
    private bool isCrouching = false;
    
    private SpriteRenderer spriteRenderer;
    private float playerColorAlpha = 1;
    
    //sprint function stuff
    private bool isSprinting = false;
    private float stamina = 100;
    private bool sprintRequiresRelease = false;
    
    // Sprint transition (smooth acceleration/deceleration)
    [SerializeField] private float sprintTransitionDuration = 0.2f;
    private float sprintBlend = 0f; // 0 = no sprint, 1 = full sprint
    private float targetSprintBlend = 0f;

    // Cached consumable modifiers (recalculated on effects change)
    private float cachedSpeedMultiplier = 1f;
    private float cachedAccelerationMultiplier = 1f;
    private float cachedMaxHealthBonus = 0f;
    private float cachedHealthPerSecond = 0f;
    private float cachedMaxStaminaBonus = 0f;
    private float cachedStaminaPerSecond = 0f;
    private float cachedDamageTakenMultiplier = 1f;
    private float cachedNoiseMultiplier = 1f;
    private float cachedWeaponSpreadMultiplier = 1f;
    
    //Thing I need for animations and sfx - Bartek
    [FormerlySerializedAs("animator")] [SerializeField]
    private Animator _animator;
    private Camera _mainCam;
    private Vector3 _mousePos;
    private bool _isMoving; 
    public bool IsMoving
    {
        get => _isMoving;
    }

    // Physics
    private Rigidbody2D rb;
    
    
             private void Awake()
             {
                 playerInput = GetComponent<PlayerInput>();
                 rb = GetComponent<Rigidbody2D>();
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
        
        _mainCam = Camera.main;
        if (_mainCam == null)
        {
            _mainCam = FindFirstObjectByType<Camera>();    
        }
        
        friction = 1-friction;
        spriteRenderer = GetComponent<SpriteRenderer>();

        _lastKnownHealth = currentHealth;
        float healthPercent = GetCurrentHealthPercent();
        onHealthChanged?.Invoke(healthPercent);

        _lastKnownStamina = stamina;
        float staminaPercent = (maxStamina > 0) ? (float)stamina / maxStamina : 0;
        onStaminaChanged?.Invoke(staminaPercent);

        // Initialize stamina mirror and subscribe to consumable changes
        staminaFloat = stamina;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnConsumableEffectsChanged += RecalculateConsumableCaches;
        }

        // Initial cache calculation
        RecalculateConsumableCaches();
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
         UpdateSprintBlend();
         if (!isRolling)
         {
             CalculateSpeed();
         }
         Move();
         UpdateAnimations();
     }

    private void ManageStamina()
    {
        float effectiveMaxStamina = GetEffectiveMaxStamina();
        if (stamina != _lastKnownStamina)
        {
            _lastKnownStamina = stamina;
            
            float staminaPercent = (effectiveMaxStamina > 0) ? (float)stamina / effectiveMaxStamina : 0;
            
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
                    _animator.SetTrigger("Die");
                }
                else
                {
                    _animator.SetTrigger("Hurt");
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
         
         if(sprintAction.triggered && stamina > 30)
         {
             isSprinting = true;
         }
         
         if(!sprintAction.IsPressed())
         {
             isSprinting = false;
         }
         if (!sprintAction.IsPressed())
         {
             sprintRequiresRelease = false;
         }

         // Set target sprint blend based on input, stamina and the release lock.
         if (sprintAction.IsPressed() && stamina > 0 && !sprintRequiresRelease)
         {
             targetSprintBlend = 1f;
         }
         else
         {
             targetSprintBlend = 0f;
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

          if (InventoryManager.Instance)
          {
              float currentWeight = InventoryManager.Instance.GetTotalWeight();
              
              weightPenaltyMultiplier = Mathf.Max(minimumSpeedPercentage, 1f - (currentWeight * speedPenaltyPerPoint));
          }

          // Calculate sprint multiplier based on smooth blend
          float sprintMultiplier = Mathf.Lerp(1f, sprintPower, sprintBlend);
          
          float currentEffectiveAcceleration = 
              acceleration * weightPenaltyMultiplier * consumableAccelerationMultiplier * sprintMultiplier;
          float currentEffectiveSpeed = speed * weightPenaltyMultiplier * consumableSpeedMultiplier * sprintMultiplier;

          // Apply friction (dampening)
          currentSpeed *= friction;
          
          // Add acceleration based on input (without Time.fixedDeltaTime - rb.velocity handles that)
          currentSpeed += moveInput * currentEffectiveAcceleration;
          
          // Clamp to max speed
          float currentSpeedMagnitude = currentSpeed.magnitude;
          if (currentSpeedMagnitude > currentEffectiveSpeed)
          {
              currentSpeed = currentSpeed.normalized * currentEffectiveSpeed;
          }
      }

     void Move() 
     {
         if (rb)
         {
             rb.linearVelocity = currentSpeed;
         }
     }

    void UpdateAnimations()
    {
        if (_mainCam == null || Mouse.current == null) return;
        
        _isMoving = Mathf.Abs(currentSpeed.x) > 1f || Mathf.Abs(currentSpeed.y) > 1f;
        _animator.SetBool("isWalking", _isMoving);
        
        Vector3 mouseScreenPos = (Vector3)Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(_mainCam.transform.position.z - transform.position.z);

        Vector2 direction = (_mainCam.ScreenToWorldPoint(mouseScreenPos) - transform.position).normalized;
        
        _animator.SetFloat("XInput", direction.x);
        _animator.SetFloat("YInput", direction.y);
    }

    public void SetWeaponAnimation(int weaponID)
    {
        if (_animator != null)
        {
            _animator.SetInteger("WeaponID", weaponID);
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
                 targetSprintBlend = 0f;
                 isSprinting = false;
                 sprintRequiresRelease = true;
             }

                stamina = Mathf.Max(0, stamina - 1);
                staminaFloat = stamina;
         }
         else
         {
                float effMaxStamina = GetEffectiveMaxStamina();
                if (stamina < effMaxStamina)
                {
                    stamina = Mathf.Min((int)Mathf.Floor(effMaxStamina), stamina + 0.5f);
                    staminaFloat = stamina;
                }
         }
     }
     
     void UpdateSprintBlend()
     {
         // Interpolate sprint blend smoothly towards target
         float maxChange = (1f / sprintTransitionDuration) * Time.fixedDeltaTime;
         sprintBlend = Mathf.MoveTowards(sprintBlend, targetSprintBlend, maxChange);
     }
     
     public void ClearInputState()
     {
         moveInput = Vector2.zero;
         currentSpeed = Vector2.zero;
         isSprinting = false;
         targetSprintBlend = 0f;
         sprintBlend = 0f;
         sprintRequiresRelease = false;
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

        // Stamina: clamp to effective max and apply consumable stamina per second
        float effectiveMaxStamina = GetEffectiveMaxStamina();
        if (staminaFloat > effectiveMaxStamina)
        {
            staminaFloat = effectiveMaxStamina;
            stamina = Mathf.FloorToInt(staminaFloat);
        }

        float staminaPerSecond = GetConsumableStaminaPerSecond();
        if (staminaPerSecond > 0f)
        {
            staminaFloat = Mathf.Min(effectiveMaxStamina, staminaFloat + (staminaPerSecond * Time.fixedDeltaTime));
            stamina = Mathf.Clamp(Mathf.FloorToInt(staminaFloat), 0, Mathf.FloorToInt(effectiveMaxStamina));
        }
    }

    private float GetCurrentHealthPercent()
    {
        float effectiveMaxHealth = Mathf.Max(0.01f, GetEffectiveMaxHealth());
        return currentHealth / effectiveMaxHealth;
    }

    private float GetEffectiveMaxHealth()
    {
        return Mathf.Max(1f, maxHealth + cachedMaxHealthBonus);
    }

    private float GetConsumableSpeedMultiplier()
    {
        return Mathf.Max(0f, cachedSpeedMultiplier);
    }

    private float GetConsumableAccelerationMultiplier()
    {
        return Mathf.Max(0f, cachedAccelerationMultiplier);
    }

    private float GetConsumableHealthPerSecond()
    {
        return Mathf.Max(0f, cachedHealthPerSecond);
    }

    private float GetEffectiveMaxStamina()
    {
        return Mathf.Max(0f, maxStamina + cachedMaxStaminaBonus);
    }

    private float GetConsumableStaminaPerSecond()
    {
        return Mathf.Max(0f, cachedStaminaPerSecond);
    }

    // Recalculate cached consumable modifiers (called when consumable effects change)
    private void RecalculateConsumableCaches()
    {
        if (InventoryManager.Instance != null)
        {
            cachedSpeedMultiplier = InventoryManager.Instance.GetConsumableSpeedMultiplier();
            cachedAccelerationMultiplier = InventoryManager.Instance.GetConsumableAccelerationMultiplier();
            cachedMaxHealthBonus = InventoryManager.Instance.GetConsumableMaxHealthBonus();
            cachedHealthPerSecond = InventoryManager.Instance.GetConsumableHealthPerSecond();
            cachedMaxStaminaBonus = InventoryManager.Instance.GetConsumableMaxStaminaBonus();
            cachedStaminaPerSecond = InventoryManager.Instance.GetConsumableStaminaPerSecond();
            cachedDamageTakenMultiplier = InventoryManager.Instance.GetConsumableDamageTakenMultiplier();
            cachedNoiseMultiplier = InventoryManager.Instance.GetConsumableNoiseMultiplier();
            cachedWeaponSpreadMultiplier = InventoryManager.Instance.GetConsumableWeaponSpreadMultiplier();
        }
        else
        {
            cachedSpeedMultiplier = 1f;
            cachedAccelerationMultiplier = 1f;
            cachedMaxHealthBonus = 0f;
            cachedHealthPerSecond = 0f;
            cachedMaxStaminaBonus = 0f;
            cachedStaminaPerSecond = 0f;
            cachedDamageTakenMultiplier = 1f;
            cachedNoiseMultiplier = 1f;
            cachedWeaponSpreadMultiplier = 1f;
        }

        // Enforce clamps on current health and stamina when caches change
        float effMaxHealth = GetEffectiveMaxHealth();
        if (currentHealth > effMaxHealth)
        {
            currentHealth = effMaxHealth;
            float healthPercent = GetCurrentHealthPercent();
            onHealthChanged?.Invoke(healthPercent);
        }

        float effMaxStamina = GetEffectiveMaxStamina();
        if (staminaFloat > effMaxStamina)
        {
            staminaFloat = effMaxStamina;
            stamina = Mathf.FloorToInt(staminaFloat);
            float staminaPercent = (effMaxStamina > 0) ? (float)stamina / effMaxStamina : 0f;
            onStaminaChanged?.Invoke(staminaPercent);
        }
    }

    // Expose cached multipliers for other systems
    public float GetCachedWeaponSpreadMultiplier() => cachedWeaponSpreadMultiplier;
    public float GetCachedNoiseMultiplier() => cachedNoiseMultiplier;

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

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnConsumableEffectsChanged -= RecalculateConsumableCaches;
        }
    }

    protected override void Kill()
    {
        base.Kill();
        onPlayerDeath?.Invoke();
    }

    // Provide damage multiplier to base Actor (consumable-based)
    protected override float GetIncomingDamageMultiplier()
    {
        return cachedDamageTakenMultiplier;
    }

    public bool IsCrouching() => isCrouching;
    public bool IsSprinting() => isSprinting;

    // Reduce player's stamina by given integer amount (clamped to 0) and invoke UI update
    public void ReduceStamina(int amount)
    {
        if (amount <= 0) return;

        float effectiveMaxStamina = GetEffectiveMaxStamina();

        stamina = Mathf.Max(0, stamina - amount);
        staminaFloat = stamina;

        float staminaPercent = (effectiveMaxStamina > 0) ? (float)stamina / effectiveMaxStamina : 0f;
        onStaminaChanged?.Invoke(staminaPercent);
    }
    public float GetStamina() => stamina;
}