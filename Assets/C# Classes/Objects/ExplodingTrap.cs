using System;
using System.Collections;
using UnityEngine;

namespace C__Classes.Objects
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

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            tag = "trap";
            DamageTrigger.enabled = true;
            
            print("boom");
            yield return new WaitForFixedUpdate();
            Destroy(DamageTrigger);
            Destroy(gameObject, waitUntilDestroyed);
        }
    }
}