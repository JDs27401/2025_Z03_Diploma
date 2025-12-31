using UnityEngine;

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

    protected void Start()
    {
        currentHealth = maxHealth;
    }

    public void DealDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void Heal(float heal)
    {
        if (currentHealth == maxHealth)
        {
            return;
        } 
        
        if (currentHealth + heal > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += heal;
        }
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
