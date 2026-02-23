using UnityEngine;

namespace C__Classes.Objects
{
    public class ExplodingTrap : Actor
    {
        [SerializeField] private Collider2D TrapTrigger;
        [SerializeField] private Collider2D DamageTrigger;
        [SerializeField] private float DestroyTriggerAfter = 0.25f;

        private new void Start()
        {
            if (ReferenceEquals(TrapTrigger, null) || ReferenceEquals(DamageTrigger, null))
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
            // yield return new WaitForFixedUpdate();
            Destroy(DamageTrigger, 0.25f);
            Destroy(gameObject, waitUntilDestroyed);
        }
    }
}