using System.Collections.Generic;
using C__Classes.Managers;
using C__Classes.Systems;
using UnityEngine;

public class PlayerInteractionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private string radioItemId = "emergency_radio";

    private HashSet<GameObject> interactablesInRange = new HashSet<GameObject>();
    private bool radioUsed;

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

        bool holdingRadio = !radioUsed && InventoryManager.Instance != null
            && InventoryManager.Instance.GetActiveItem()?.id == radioItemId;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(interactablesInRange.Count > 0 || holdingRadio);
        }

        if (holdingRadio && Input.GetKeyDown(KeyCode.E))
        {
            radioUsed = true;
            if (JournalManager.Instance != null)
            {
                JournalManager.Instance.ShowMessage("Final wave started!");
            }
            if (Universe.Instance != null)
            {
                Universe.Instance.StartFinalWave();
            }
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