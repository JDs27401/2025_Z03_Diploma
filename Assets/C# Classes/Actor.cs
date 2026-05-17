using System;
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
        
        protected Tilemap tilemap;
        protected TilemapGenerationSystem.TileProperties currentTile;
        protected TilemapGenerationSystem tilemapGenerationSystem;

        public TileType TileType { get; set; } = TileType.Ground;

        //Bartek - this is for animator issues, as everything should inherit from Actor class
        //we want to be able how long we want to wait until deleting this object by "kill" method
        [SerializeField] protected float waitUntilDestroyed;

        protected virtual void Start()
        {
            currentHealth = maxHealth;
        }

        protected virtual void Update()
        {
            if (isDead)
            {
                return;
            }
        }

        public void DealDamage(float dmg)
        {
            if (!invulnerable)
            {
                currentHealth -= dmg;
            }
            // print("currentHealth: " + currentHealth);
            if (currentHealth <= 0)
            {
                Kill();
            }
        }

        //function for starting the coroutine
        public void StartDot(float dmg, float duration, float interval)
        {
            if (isDead)
            {
                return;
            }
            StartCoroutine(Dot(dmg, duration, interval));
        }
        
        //Damage Over Time implementation, coroutine is being started by StartDot() which means it is started from this script, which will not result in Exception while being destroyed while still running
        private IEnumerator Dot(float dmg, float duration, float interval)
        {
            for (float i = 0; i < duration; i += interval)
            {
                DealDamage(dmg);
                yield return new WaitForSeconds(interval);
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

        protected virtual void Kill()
        {
            isDead = true;
            Destroy(gameObject, waitUntilDestroyed);
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

        public float GetWaitUntilDestroyed()
        {
            return waitUntilDestroyed;
        }
        public void SetWaitUntilDestroyed(float wait)        
        {
            waitUntilDestroyed = wait;
        }

        public float GetDamage()
        {
            return damage;
        }
        
        public void SetDamage(float value)
        {
            damage = value;
        }
    }
}