using System;
using UnityEngine;

public class FOVScript : MonoBehaviour
{
    private EAI parentAI;
    
    void Start()
    {
        parentAI = GetComponentInParent<EAI>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player") && !other.GetComponent<Movement>().IsCrouching())
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