using Unity.VisualScripting;
using UnityEngine;

public class FOVScript : MonoBehaviour
{
    private AI callParentAI;
    void Start()
    {
        callParentAI = GetComponentInParent<AI>();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        Vector2 collisiionPoint = other.gameObject.transform.position;
        callParentAI.Aggrevate(collisiionPoint);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        callParentAI.Pacify();
    }
}
