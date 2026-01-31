using UnityEngine;

public class FOVScript : MonoBehaviour
{
    private EAI parentAI;
    
    void Start()
    {
        parentAI = GetComponentInParent<EAI>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
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