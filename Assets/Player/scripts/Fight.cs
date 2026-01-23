using System.Collections;
using C__Classes; 
using UnityEngine;
using UnityEngine.InputSystem;

public class Fight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // jak null to pociski będą wylatywać ze środka gracza

    [Header("Environment")]
    [SerializeField] private LayerMask obstacleLayer; // warstwa ścian

    [Header("Melee Stats")]
    [SerializeField] private float meleeSpeed = 2f; // hits per sec 
    [SerializeField] private float meleeRange = 2.5f;
    [SerializeField] private float meleeDamage = 25f;
    [Range(0, 360)] [SerializeField] private float meleeAngle = 120f; 
    [SerializeField] private Material meleeVisualMaterial;

    [Header("Visuals Settings")]
    [SerializeField] private int visualSortingOrder = 10;

    [Header("Ranged Stats")]
    [SerializeField] private float shootingSpeed = 4f; // shots per sec
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileSize = 0.5f;
    [SerializeField] private float projectileSpread = 5f; // losowy rozrzut (stopnie)
    [SerializeField] private float projectileDamage = 10f; 

    private float nextMeleeTime = 0f;
    private float nextFireTime = 0f;

    private Camera mainCam;
    private Animator animator;

    // do wizualizacji ataku
    private GameObject meleeVisualObj;
    private MeshFilter meleeMeshFilter;
    private MeshRenderer meleeMeshRenderer;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) mainCam = FindFirstObjectByType<Camera>();
        animator = GetComponent<Animator>();
        if (firePoint == null) firePoint = transform;

        InitMeleeVisualizer();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + (1f / shootingSpeed);
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Time.time >= nextMeleeTime)
            {
                MeleeAttack();
                nextMeleeTime = Time.time + (1f / meleeSpeed);
            }
        }
    }

    void MeleeAttack()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector2 directionToMouse = (mouseWorldPos - transform.position).normalized;

        StartCoroutine(ShowMeleeVisuals(directionToMouse));

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("hostile"))
            {
                Vector2 directionToEnemy = (hit.transform.position - transform.position).normalized;
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                float angleToEnemy = Vector2.Angle(directionToMouse, directionToEnemy);
                if (angleToEnemy < meleeAngle / 2f)
                {
                    RaycastHit2D hitWall = Physics2D.Raycast(transform.position, directionToEnemy, distanceToEnemy, obstacleLayer);

                    if (hitWall.collider == null) 
                    {
                        Actor enemyActor = hit.GetComponent<Actor>();
                        if (enemyActor != null)
                        {
                            enemyActor.DealDamage(meleeDamage);
                        }
                    }
                    else
                    {
                        Debug.DrawLine(transform.position, hitWall.point, Color.red, 1f);
                    }
                }
            }
        }
    }

    void InitMeleeVisualizer()
    {
        meleeVisualObj = new GameObject("MeleeHitboxVisualizer");
        meleeVisualObj.transform.SetParent(transform);
        meleeVisualObj.transform.localPosition = Vector3.zero;
        
        meleeMeshFilter = meleeVisualObj.AddComponent<MeshFilter>();
        meleeMeshRenderer = meleeVisualObj.AddComponent<MeshRenderer>();
        
        meleeMeshRenderer.sortingOrder = visualSortingOrder; 
        
        meleeMeshRenderer.enabled = false;
        
        if (meleeVisualMaterial != null)
        {
            meleeMeshRenderer.material = meleeVisualMaterial;
        }
        else
        {
            Material tempMat = new Material(Shader.Find("Sprites/Default"));
            tempMat.color = new Color(1, 0, 0, 0.3f);
            meleeMeshRenderer.material = tempMat;
        }
    }

    IEnumerator ShowMeleeVisuals(Vector2 direction)
    {
        Mesh mesh = CreateArcMesh();
        meleeMeshFilter.mesh = mesh;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        meleeVisualObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        meleeMeshRenderer.enabled = true;
        yield return new WaitForSeconds(0.15f);
        meleeMeshRenderer.enabled = false;
    }

    Mesh CreateArcMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 20; 
        int numVertices = segments + 2; 
        
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float currentAngle = -meleeAngle / 2f;
        float angleStep = meleeAngle / segments;

        for (int i = 1; i < numVertices; i++)
        {
            float rad = (currentAngle + 90) * Mathf.Deg2Rad; 
            vertices[i] = new Vector3(Mathf.Cos(rad) * meleeRange, Mathf.Sin(rad) * meleeRange, 0);
            currentAngle += angleStep;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;           
            triangles[i * 3 + 1] = i + 1;   
            triangles[i * 3 + 2] = i + 2;   
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float randomSpread = Random.Range(-projectileSpread, projectileSpread);
        Quaternion rotation = Quaternion.Euler(0, 0, rotZ + randomSpread);

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, rotation);
        
        Projectile projScript = bullet.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Setup(projectileSpeed, projectileDamage, projectileSize);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        return mainCam.ScreenToWorldPoint(mouseScreenPos);
    }
}