﻿using System.Collections;
using C__Classes.Objects;
using Player.scripts;
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
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            BaseDamagePipelineFunction(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("attack"))
            {
                return;
            }
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
                        
                        // case "attack":
                        //     // movement.DealDamage(otherActor.GetDamage());
                        //     _self.DealDamage(otherActor.GetDamage());
                        //     _self.StartInvulnerability();
                        //     TrySpawnDotAreaFromAttack(other);
                        //     break;
                        
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
                            TrySpawnDotAreaFromAttack(other);
                            break;
                        
                        case "trap":
                            if (_lastMine == other.gameObject) return;
                            _lastMine = other.gameObject;
                            
                            _self.DealDamage(otherActor.GetDamage());
                            TrySpawnDotAreaFromAttack(other);
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

        //@todo verify if this function is needed or can be deleted
        private void TrySpawnDotAreaFromAttack(Collider2D attackCollider)
        {
            if (attackCollider == null) return;

            MeleeAttackProperties props = attackCollider.GetComponent<MeleeAttackProperties>();
            if (props == null || !props.isMolotov) return;

            // spawn dot area at point closest to this object
            Vector3 spawnPos = attackCollider.ClosestPoint(transform.position);

            GameObject dotArea = new GameObject("MolotovDotArea");
            dotArea.transform.position = spawnPos;
            
            Rigidbody2D rb = dotArea.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            
            CircleCollider2D trigger = dotArea.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = props.dotAreaRadius;

            Actor areaActor = dotArea.AddComponent<Actor>();
            areaActor.SetDamage(props.dotDamage);
            areaActor.SetWaitUntilDestroyed(props.dotAreaLifetime);

            DotComponent dotComponent = dotArea.AddComponent<DotComponent>();
            dotComponent.Configure(trigger, props.dotAreaRadius, props.dotDuration, props.dotInterval);
            dotComponent.StartDotArea();
        }
    }
}