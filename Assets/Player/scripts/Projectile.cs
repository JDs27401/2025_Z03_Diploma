using C__Classes; // Do dostępu do klasy Actor
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private Vector2 direction;

    public void Setup(float projectileSpeed, float projectileDamage, float projectileSize)
    {
        speed = projectileSpeed;
        damage = projectileDamage;
        
        // Ustawiamy skalę pocisku
        transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        
        // Usuń pocisk po 5 sekundach, żeby nie zaśmiecać pamięci, jeśli w nic nie trafi
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Poruszanie się pocisku "do przodu" względem własnej rotacji
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorujemy kolizje z graczem i innymi pociskami/znajdźkami
        if (other.CompareTag("Player") || other.CompareTag("projectile") || other.CompareTag("heal")) 
            return;

        // Jeśli trafiliśmy w coś, co jest ścianą (np. TilemapCollider) - niszczymy pocisk
        if (!other.isTrigger && (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.CompareTag("Untagged")))
        {
            Destroy(gameObject);
            return;
        }

        // Jeśli trafiliśmy w przeciwnika
        if (other.CompareTag("hostile"))
        {
            Destroy(gameObject); // Zniszcz pocisk po trafieniu
        }
    }
}