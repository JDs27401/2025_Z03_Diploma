using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Objects
{
    public class NoiseStimuliComponent : MonoBehaviour
    {
        private NpcBase _aiController;

        private void Start()
        {
            _aiController = GetComponent<NpcBase>();
            if (_aiController == null)
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
            _aiController.Aggravate(FindFirstObjectByType<PlayerController>().transform);
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
            
            _aiController.Pacify();
            _aiController.Agent.SetDestination(transform.position);
        }
    }
}