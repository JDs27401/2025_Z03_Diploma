using System;
using Enemy.Scripts;
using UnityEngine;

namespace Enemy.Scripts
{
    public class FOVScript : MonoBehaviour
    {
        private NpcBase parentAI;
    
        void Start()
        {
            parentAI = GetComponentInParent<NpcBase>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("player") && !other.GetComponent<PlayerController>().IsCrouching())
            {
                parentAI.Aggravate(other.transform);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("player"))
            {
                parentAI.Pacify();
            }
        }
    }
}