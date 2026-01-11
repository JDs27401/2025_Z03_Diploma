using System.Collections;
using UnityEngine;

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

        protected void Start()
        {
            currentHealth = maxHealth;
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