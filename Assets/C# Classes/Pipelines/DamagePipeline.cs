using UnityEngine;

namespace C__Classes.Pipelines
{
    public class DamagePipeline : MonoBehaviour
    {
        // private Collider2D collision;
        private Actor self;

        private void Awake()
        {
            // collision = FindFirstObjectByType<Collider2D>();
            self = GetComponent<Actor>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            /*Debug.Log($"Trigger {gameObject.name} hit by {other.name}");

            if (CompareTag("heal") && other.CompareTag("player"))
            {
                Debug.Log("Healing player");
                other.GetComponent<Actor>()?.Heal(100);
            }*/
            
            if (CompareTag("heal"))
            {
                if (other.CompareTag("player"))
                {
                    Actor target = other.GetComponent<Actor>();
                    if (target == null)
                    {
                        return;
                    }
                    target.Heal(100);
                }
            }

            if (CompareTag("hostile"))
            {
                if (other.CompareTag("player"))
                {
                    Actor target = other.GetComponent<Actor>();
                    if (target == null)
                    {
                        return;
                    }
                    other.GetComponent<Actor>().DealDamage(self.GetDamage());
                }    
            }
        }
    }
}