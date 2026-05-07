using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt; 
    
    private HashSet<GameObject> interactablesInRange = new HashSet<GameObject>();

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        interactablesInRange.RemoveWhere(item => item == null);

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(interactablesInRange.Count > 0);
        }
    }

    public void AddInteractable(GameObject interactable)
    {
        interactablesInRange.Add(interactable);
    }

    public void RemoveInteractable(GameObject interactable)
    {
        interactablesInRange.Remove(interactable);
    }
}