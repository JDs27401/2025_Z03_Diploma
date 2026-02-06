using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArcHitbox : MonoBehaviour
{
    private PolygonCollider2D polyCollider;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    [Header("Visual Settings")]
    [Tooltip("Materiał wizualizacji (np. półprzezroczysty czerwony)")]
    [SerializeField] private Material visualMaterial;
    [Tooltip("Kolejność rysowania (np. aby było nad podłogą)")]
    [SerializeField] private int sortingOrder = 10;

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        polyCollider.isTrigger = true;
        
        // Konfiguracja renderera
        meshRenderer.sortingOrder = sortingOrder;
        
        // Jeśli nie przypisano materiału w inspektorze, stwórz domyślny
        if (visualMaterial != null)
        {
            meshRenderer.material = visualMaterial;
        }
        else
        {
            // Awaryjny materiał (czerwony, półprzezroczysty)
            Material defaultMat = new Material(Shader.Find("Sprites/Default"));
            defaultMat.color = new Color(1f, 0f, 0f, 0.4f); // R=1, G=0, B=0, Alpha=0.4
            meshRenderer.material = defaultMat;
        }
    }

    public void SetArcShape(float angle, float radius, int segments = 20)
    {
        if (polyCollider == null) polyCollider = GetComponent<PolygonCollider2D>();

        // 1. Obliczanie punktów (tak jak wcześniej)
        Vector2[] points = new Vector2[segments + 2];
        points[0] = Vector2.zero; // Środek

        float startAngle = -angle / 2f;
        float angleStep = angle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngleDeg = startAngle + (angleStep * i);
            float currentAngleRad = currentAngleDeg * Mathf.Deg2Rad;

            float x = Mathf.Cos(currentAngleRad) * radius;
            float y = Mathf.Sin(currentAngleRad) * radius;

            points[i + 1] = new Vector2(x, y);
        }

        // 2. Aktualizacja Collidera
        polyCollider.SetPath(0, points);

        // 3. Generowanie wizualnego Mesha
        GenerateMesh(points);
    }

    private void GenerateMesh(Vector2[] points2D)
    {
        Mesh mesh = new Mesh();

        // Konwersja Vector2[] na Vector3[] dla Mesha
        Vector3[] vertices = new Vector3[points2D.Length];
        for (int i = 0; i < points2D.Length; i++)
        {
            vertices[i] = (Vector3)points2D[i];
        }

        // Tworzenie trójkątów (wachlarz)
        int triangleCount = points2D.Length - 2;
        int[] triangles = new int[triangleCount * 3];

        for (int i = 0; i < triangleCount; i++)
        {
            // Każdy trójkąt łączy środek (0) z dwoma kolejnymi punktami na obwodzie
            triangles[i * 3] = 0;           // Środek
            triangles[i * 3 + 1] = i + 1;   // Punkt obecny
            triangles[i * 3 + 2] = i + 2;   // Punkt następny
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        // Obliczenie normalnych i bounds (ważne dla oświetlenia i culling'u)
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }
}