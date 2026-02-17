using System;
using UnityEngine;

namespace C__Classes
{
    public class ExplodingTrap : Actor
    {
        [SerializeField] private Collider2D TrapTrigger;
        [SerializeField] private Collider2D DamageTrigger;

        private new void Start()
        {
            if (ReferenceEquals(TrapTrigger, null))
            {
                return;
            }
            DamageTrigger.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            tag = "trap";
            DamageTrigger.enabled = true;
            
            print("boom");
            Destroy(DamageTrigger);
            Destroy(gameObject, waitUntilDestroyed);
        }
    }
}