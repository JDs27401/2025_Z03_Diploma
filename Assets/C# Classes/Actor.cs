using System.Collections;
using C__Classes.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace C__Classes
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] //max speed stat
        protected float speed = 4f;
        [SerializeField] //how fast will player gain speed
        protected float acceleration = 2f;
        [SerializeField] //how fast will the player slow down (the higher the more friction)
        protected float friction = 0.10f;

        [SerializeField]
        protected float maxHealth = 100f;
        [SerializeField]
        protected float currentHealth;

        [SerializeField] 
        protected float damage = 0f;
        protected bool invulnerable = false;
        [SerializeField] 
        protected float iFrameTime = 1f;
        protected bool isDead = false;
        
        // protected Vector3Int worldPosition;
        [SerializeField]
        protected Tilemap tilemap;
        protected TilemapGenerationSystem.TileProperties currentTile;
        protected TilemapGenerationSystem tilemapGenerationSystem;

        protected void Start()
        {
            currentHealth = maxHealth;
            tilemapGenerationSystem = tilemap.GetComponent<TilemapGenerationSystem>();
            currentTile = tilemapGenerationSystem
                .GetTileProperties(tilemap.WorldToCell(transform.position).x, tilemap.WorldToCell(transform.position).y);
            // print("???");
            if (ReferenceEquals(tilemap, null) || ReferenceEquals(tilemapGenerationSystem, null))
            {
                print("tilemap albo tilegenerationsystem to null");
                return;
            }

        }

        protected void Update()
        {
            GetActorTilePosition();
        }

        public void GetActorTilePosition()
        {
            if (ReferenceEquals(tilemap, null))
            {
                print("tilemap to null");
                return;
            }
            
            // worldPosition = tilemap.WorldToCell(transform.position);
            TilemapGenerationSystem.TileProperties newTile = tilemapGenerationSystem.GetTileProperties(tilemap.WorldToCell(transform.position).x, tilemap.WorldToCell(transform.position).y);
            // print(tilemapGenerationSystem.GetTileProperties(tilemap.WorldToCell(transform.position).x, tilemap.WorldToCell(transform.position).y));
            // print(newTile);
            
            if (newTile.type == currentTile.type)
            {
                return;
            }
            
            currentTile = newTile;
            //reszta kodu odpowiedzialnego za movement
        }

        public void DealDamage(float dmg)
        {
            if (!invulnerable)
            {
                currentHealth -= dmg;
            }
            if (currentHealth <= 0)
            {
                Kill();
            }
        }

        public void Heal(float heal)
        {
            if (currentHealth + heal >= maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += heal;
            }
        }

        private IEnumerator iFrame()
        {
            invulnerable = true;
            yield return new WaitForSeconds(iFrameTime);
            invulnerable = false;
        }

        public void StartInvulnerability()
        {
            StartCoroutine(iFrame());
        }

        public void Kill()
        {
            isDead = true;
            Destroy(gameObject);
        }

        public bool GetInvulnerable()
        {
            return invulnerable;
        }

        // public getters
        public float GetSpeed()
        {
            return speed;
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        public float GetAcceleration()
        {
            return acceleration;
        }

        public float GetFriction()
        {
            return friction;
        }

        public float GetDamage()
        {
            return damage;
        }
    }
}