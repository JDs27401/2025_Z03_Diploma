using C__Classes; // Do dostępu do klasy Actor
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private Vector2 direction;
    private bool isExplosive;
    private float explosionRadius;
    private bool hasCollided;

    public void Setup(float projectileSpeed, float projectileDamage, float projectileSize, bool explosive = false, float expRadius = 0f)
    {
        speed = projectileSpeed;
        damage = projectileDamage;
        isExplosive = explosive;
        explosionRadius = expRadius;
        
        transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (hasCollided)
        {
            return;
        }
        
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private bool ShouldIgnoreCollision(Collider2D other)
    {
        if (isExplosive && !CompareTag("trap"))
        {
            if (other.CompareTag("player") || other.CompareTag("Player"))
            {
                return true;
            }

            Transform current = other.transform;
            while (current != null)
            {
                if (current.CompareTag("player") || current.CompareTag("Player"))
                {
                    return true;
                }
                current = current.parent;
            }
            
        }

        return other.CompareTag("projectile") || other.CompareTag("heal");
    }

    private void StopProjectile()
    {
        hasCollided = true;
        speed = 0f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isExplosive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
