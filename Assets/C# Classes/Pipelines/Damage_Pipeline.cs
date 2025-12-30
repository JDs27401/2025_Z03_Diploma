using UnityEngine;

namespace C__Classes.Pipelines
{
    public class Damage_Pipeline : MonoBehaviour
    {
        private Collider2D collision;

        private void Start()
        {
            collision = FindFirstObjectByType<Collider2D>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Actor>().DealDamage(10);
            }
        }
    }
}