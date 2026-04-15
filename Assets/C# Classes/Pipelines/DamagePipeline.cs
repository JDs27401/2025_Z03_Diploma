using System.Collections;
using C__Classes.Objects;
using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Pipelines
{
    public class DamagePipeline : MonoBehaviour
    {
        private Actor _self;
        
        private GameObject _lastMine = null;
        private bool _canTakeDamageFromDot = true;

        private void Awake()
        {
            _self = GetComponent<Actor>();
            //@todo implement case for DOT effect in DMG pipeline - new tag required
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
            if (CompareTag(other.tag))
            {
                return;
            }
            Actor otherActor = other.GetComponent<Actor>();
            if (!otherActor) //fix for script being activated if entity entering does not have Actor
            {
                return;
            }
            
            if (_self.GetInvulnerable())
            {
                return;
            }
            //yes, the branches are the same, but this is on purpose for now, if we want to do different things depending on the trigger type
            switch (tag)
            {
                case "player": //this list interaction that player tagged object can interact with
                    // Movement movement = GetComponent<Movement>();
                    
                    switch (other.tag)
                    {
                        case "hostile":
                            // movement.DealDamage(otherActor.GetDamage());
                            if (other.GetComponent<ExplodingEnemy>())
                            {
                                return;
                            }
                            _self.DealDamage(otherActor.GetDamage());
                            _self.StartInvulnerability();
                            break;
                        
                        //workaround for now, so that player does not get damaged from its own bullets
                        // case "projectile":
                        //     movement.DealDamage(otherActor.GetDamage());
                        //     self.DealDamage(otherActor.GetDamage());
                        //     self.StartInvulnerability();
                        //     break;
                        
                        case "attack":
                            // movement.DealDamage(otherActor.GetDamage());
                            _self.DealDamage(otherActor.GetDamage());
                            _self.StartInvulnerability();
                            break;
                        
                        case "trap":
                            if (_lastMine == other.gameObject) return;
                            _lastMine = other.gameObject;
                            
                            // movement.DealDamage(otherActor.GetDamage());
                            _self.DealDamage(otherActor.GetDamage());
                            _self.StartInvulnerability();
                            break;
                        
                        case "heal":
                            // movement.DealDamage(otherActor.GetDamage());
                            _self.Heal(otherActor.GetDamage());
                            break;
                        
                        case "dot":
                            if (!_canTakeDamageFromDot)
                            {
                                return;
                            }
                            _self.DealDamage(otherActor.GetDamage());
                            StartCoroutine(HandleDotCooldown());
                            break;
                    }
                    break;
                
                case "hostile": //this list interaction that hostile tagged object can interact with
                    switch (other.tag)
                    {
                        case "projectile":
                            _self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "attack":
                            _self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "trap":
                            if (_lastMine == other.gameObject) return;
                            _lastMine = other.gameObject;
                            
                            _self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "dot":
                            if (!_canTakeDamageFromDot)
                            {
                                return;
                            }
                            _self.DealDamage(otherActor.GetDamage());
                            StartCoroutine(HandleDotCooldown());
                            break;
                    }
                    break;
                
                case "destructible": //this list interaction that destructible tagged object can interact with
                    switch (other.tag)
                    {
                        case "projectile":
                            _self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "attack":
                            _self.DealDamage(otherActor.GetDamage());
                            break;
                        
                        case "dot":
                            if (!_canTakeDamageFromDot)
                            {
                                return;
                            }
                            _self.DealDamage(otherActor.GetDamage());
                            StartCoroutine(HandleDotCooldown());
                            break;
                    }
                    break;
            }
        }

        private IEnumerator HandleDotCooldown()
        {
            _canTakeDamageFromDot = false;
            yield return new WaitForSeconds(1f);
            _canTakeDamageFromDot = true;
        }
    }
}