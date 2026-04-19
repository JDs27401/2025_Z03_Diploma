using System.Collections.Generic;
using UnityEngine;

namespace C__Classes.Objects
{
    public class DotComponent : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D trigger;
        [SerializeField] private float area;
        [SerializeField] private float duration;
        [SerializeField] private float interval;
        private Actor _actor;
        
        private void Start()
        {
            _actor = GetComponent<Actor>();
            if (_actor == null)
            {
                #if UNITY_EDITOR
                print("No NPC Base component found");
                #endif
                return;
            }

            if (trigger == null)
            {
                #if UNITY_EDITOR
                print("No area trigger found");
                #endif
                return;       
            }
            //
            // trigger.radius = 0f;
            // trigger.enabled = false;
            // tag = "";
        }

        public void Configure(CircleCollider2D areaTrigger, float areaRadius, float dotDuration, float dotInterval)
        {
            // print("trigger = " + areaTrigger + " areaRadius = " + areaRadius + " dotDuration = " + dotDuration + " dotInterval = " + dotInterval);
            trigger = areaTrigger;
            area = areaRadius;
            duration = dotDuration;
            interval = dotInterval;
            _actor = GetComponent<Actor>();
        }

        public void StartDotArea()
        {
            // print("Starting DOT area");
            // print(trigger.radius);
            // print(area);
            trigger.enabled = true;
            trigger.radius = area;
            tag = "dot";
            Destroy(gameObject, _actor.GetWaitUntilDestroyed());
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Actor actor = other.GetComponent<Actor>();
            if (actor == null)
            {
                return;
            }
            
            actor.StartDot(_actor.GetDamage(), duration, interval);
        }
    }
}