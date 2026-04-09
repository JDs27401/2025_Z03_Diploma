using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Objects
{
    public class NoiseStimuliComponent : MonoBehaviour
    {
        private NpcBase _base;

        private void Start()
        {
            _base = GetComponent<NpcBase>();
            if (_base == null)
            {
                #if UNITY_EDITOR
                print("No NPC Base component found");
                #endif
                return;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            #if UNITY_EDITOR
            print("Entered Noise Component");
            #endif
            _base.Aggravate(FindFirstObjectByType<PlayerController>().transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("PlayerNoiseComponent"))
            {
                return;
            }
            #if UNITY_EDITOR
            print("Left Noise Component");
            #endif
            
            _base.Pacify();
            _base.Agent.SetDestination(transform.position);
        }
    }
}