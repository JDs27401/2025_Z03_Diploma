using System;
using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Objects
{
    public class VisualStimuliComponent : MonoBehaviour
    {
        private NpcBase _aiController;

        //todo: refactor Start() into an abstract class that would be used both by VSC and NSC 
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
            if (!other.CompareTag("PlayerVisualComponent"))
            {
                return;
            }
            #if UNITY_EDITOR
            print("Aggravated on player");
            #endif
            print(other);
            _aiController.Aggravate(other.transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("PlayerVisualComponent"))
            {
                return;
            }
            #if UNITY_EDITOR
            print("Player escaped");
            #endif
            _aiController.Pacify();
        }
    }
}