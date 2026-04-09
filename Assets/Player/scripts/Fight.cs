using UnityEngine;
using UnityEngine.InputSystem;
using C__Classes; // Potrzebne do dostępu do Actor

public class Fight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint; // Punkt wylotu pocisków/ataku
    
    [Header("Prefabs")]
    [Tooltip("Prefab pocisku. Musi posiadać Collider2D(Trigger), Actor, DamagePipeline oraz tag 'projectile'.")]
    [SerializeField] private GameObject projectilePrefab;
    
    [Tooltip("Prefab ataku wręcz. Musi posiadać Collider2D(Trigger), Actor, DamagePipeline oraz tag 'attack'.")]
    [SerializeField] private GameObject meleeHitboxPrefab;

    [Header("Melee Stats")]
    [SerializeField] private float meleeSpeed = 2f; // ataki na sekundę
    [SerializeField] private float meleeDamage = 25f;
    [SerializeField] private float meleeDuration = 1f; // Jak długo hitbox istnieje na scenie
    [SerializeField] private float meleeAngle = 120;
    [SerializeField] private float meleeRange = 1f;

    [Header("Ranged Stats")]
    [SerializeField] private float shootingSpeed = 4f; // strzały na sekundę
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileSpread = 5f; // rozrzut w stopniach

    private float nextMeleeTime = 0f;
    private float nextFireTime = 0f;

    private Camera mainCam;
    // private Animator animator; // Opcjonalnie, jeśli chcesz animować postać

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) mainCam = FindFirstObjectByType<Camera>();
        
        // animator = GetComponent<Animator>();
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Strzelanie (LPM)
        if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + (1f / shootingSpeed);
            }
        }

        // Atak wręcz (PPM)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            print("a");
            if (Time.time >= nextMeleeTime)
            {
                print("b");
                MeleeAttack();
                nextMeleeTime = Time.time + (1f / meleeSpeed);
            }
        }
    }

    void MeleeAttack()
    {
        if (meleeHitboxPrefab == null) return;

        // 1. Oblicz rotację w stronę myszy
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
        // Obiekt obracamy tak, aby jego "przód" (oś X) celował w myszkę
        Quaternion rotation = Quaternion.Euler(0, 0, rotZ);

        // 2. Instancjonuj
        print("instantiate");
        GameObject hitbox = Instantiate(meleeHitboxPrefab, firePoint.position, rotation);
        hitbox.transform.SetParent(this.transform); // Przyczep do gracza

        // 3. SKONFIGURUJ KSZTAŁT (To jest nowość)
        ArcHitbox arcScript = hitbox.GetComponent<ArcHitbox>();
        if (arcScript != null)
        {
            // Przekazujemy parametry z Fight.cs do hitboxa
            arcScript.SetArcShape(meleeAngle, meleeRange);
        }

        // 4. Skonfiguruj obrażenia
        Actor actorScript = hitbox.GetComponent<Actor>();
        if (actorScript != null)
        {
            actorScript.SetDamage(meleeDamage);
        }

        // 5. Zniszcz po czasie
        Destroy(hitbox, meleeDuration);
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        // Oblicz rotację z rozrzutem
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float randomSpread = Random.Range(-projectileSpread, projectileSpread);
        Quaternion rotation = Quaternion.Euler(0, 0, rotZ + randomSpread);

        // 1. Stwórz pocisk
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, rotation);

        // 2. Skonfiguruj obrażenia
        SetupDamageOnObject(bullet, projectileDamage);

        // 3. Nadaj prędkość (jeśli pocisk ma Rigidbody2D)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = bullet.transform.right * projectileSpeed; // lub bullet.transform.up zależnie od sprite'a
        }
    }

    // Pomocnicza funkcja do ustawiania obrażeń na komponencie Actor
    private void SetupDamageOnObject(GameObject obj, float dmgValue)
    {
        Actor actorScript = obj.GetComponent<Actor>();
        if (actorScript != null)
        {
            actorScript.SetDamage(dmgValue);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        return mainCam.ScreenToWorldPoint(mouseScreenPos);
    }
}