using System.Collections;
using C__Classes.Objects;
using UnityEngine;

namespace C__Classes.Pipelines
{
    public class DamagePipeline : MonoBehaviour
    {
        private Actor self;
        
        private bool takenDamageFromMine = false;
        [SerializeField] private float MineInvurnerabilityTime = 0.5f;

        private void Awake()
        {
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
            if (!otherActor) //fix for script being activated if entity entering does not have Actor
            {
                return;
            }
            
            if (self.GetInvulnerable())
            {
                return;
            }
            
            //yes, the branches are the same, but this is on purpose for now, if we want to do different things depending on the trigger type
            switch (tag)
            {
                case "player": //this list interaction that player tagged object can interact with
                    
                    switch (other.tag)
                    {
                        case "hostile":
                            self.DealDamage(otherActor.GetDamage());
                            self.StartInvulnerability();
                            break;
                        
                        //workaround for now, so that player does not get damaged from its own bullets
                        // case "projectile":
                        //     self.DealDamage(otherActor.GetDamage());
                        //     self.StartInvulnerability();
                        //     break;
                        
                        case "attack":
                            self.DealDamage(otherActor.GetDamage());
                            self.StartInvulnerability();
                            break;
                        
                        case "trap":
                            if (takenDamageFromMine) return;
                            
                            self.DealDamage(otherActor.GetDamage());
                            self.StartInvulnerability();
                            StartCoroutine(MineInvurnerability());
                            break;
                        
                        case "heal":
                            self.Heal(otherActor.GetDamage());
                            break;
                    }
                    break;
                
                case "hostile": //this list interaction that hostile tagged object can interact with
                    switch (other.tag)
                    {
                        case "projectile":
                            self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "attack":
                            self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "trap":
                            if (takenDamageFromMine) return;
                            
                            self.DealDamage(otherActor.GetDamage());
                            StartCoroutine(MineInvurnerability());
                            break;
                    }
                    break;
                
                case "destructible": //this list interaction that destructible tagged object can interact with
                    switch (other.tag)
                    {
                        case "projectile":
                            self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "attack":
                            self.DealDamage(otherActor.GetDamage());
                            break;
                    }
                    break;
            }
        }

        private IEnumerator MineInvurnerability()
        {
            takenDamageFromMine = true;
            yield return new WaitForSeconds(MineInvurnerabilityTime);
            takenDamageFromMine = false;
        }
    }
}