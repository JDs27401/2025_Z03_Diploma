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
            BaseDamagePipelineFunction(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            BaseDamagePipelineFunction(other);
        }

        private void BaseDamagePipelineFunction(Collider2D other)
        {
            Actor otherActor = other.GetComponent<Actor>();
            if (otherActor.CompareTag("player") && 
                // !ReferenceEquals(otherActor, null) && //this is not used now
                otherActor.GetInvulnerable())
            {
                return;
            }

            //yes, the branches are the same, but this is on purpose for now, if we want to do different things depending on the trigger type
            if (other.CompareTag("player"))
            {
                switch (tag)
                {
                    case "hostile":
                        otherActor.DealDamage(self.GetDamage());
                        otherActor.StartInvulnerability();
                        //other function calls 
                        return;
                    
                    case "projectile":
                        otherActor.DealDamage(self.GetDamage());
                        otherActor.StartInvulnerability();
                        //other function calls 
                        return;
                    
                    case "attack":
                        otherActor.DealDamage(self.GetDamage());
                        otherActor.StartInvulnerability();
                        //other function calls 
                        return;
                    
                    case "trap":
                        otherActor.DealDamage(self.GetDamage());
                        otherActor.StartInvulnerability();
                        //other function calls 
                        return;
                    
                    case "heal":
                        otherActor.Heal(self.GetDamage()); //actor will be healed by the amount specified in damage field
                        //other function calls 
                        return;
                    
                    default:
                        return;
                }
            } else if (other.CompareTag("destructible"))
            {
                switch (tag)
                {
                    case "projectile":
                        otherActor.DealDamage(self.GetDamage());
                        //other function calls 
                        return;
                    
                    case "attack":
                        otherActor.DealDamage(self.GetDamage());
                        //other function calls 
                        return;
                    
                    default:
                        return;
                }
            } else if (other.CompareTag("hostile"))
            {
                switch (tag)
                {
                    case "attack":
                        otherActor.DealDamage(self.GetDamage());
                        //other function calls 
                        return;
                    
                    case "projectile":
                        otherActor.DealDamage(self.GetDamage());
                        //other function calls 
                        return;
                    
                    case "trap":
                        otherActor.DealDamage(self.GetDamage());
                        //other function calls 
                        return;
                    
                    default:
                        return;
                }
            }
        }
    }
}