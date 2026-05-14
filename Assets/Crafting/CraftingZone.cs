using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class CraftingZone : MonoBehaviour
{
    public GameObject craftingUI;

    private bool isPlayerInRange = false;
    private PlayerInteractionUI playerUI;
    
    private PlayerInput _playerInput;
    private InputAction _interactAction;
    
    private void OnDestroy()
    {
        UnsubscribeInteract();
    }
    
    //stary system detekcji interakcji - getkeydown w update
    // private void Update() 
    // {
    //     if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
    //     {
    //         if (craftingUI != null)
    //         {
    //             craftingUI.SetActive(!craftingUI.activeSelf);
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true;
            
            //nowy proponowany system interakcji - action z Player action mapa (input system)
            //plus jest taki że się to łatwo wyłącza w jednym miejscu i nie da się włączyć craftingu w menu pauzy
            //tu się podłącza do akcji w mapie
            _playerInput = other.GetComponent<PlayerInput>();
            if (_playerInput != null)
            {
                _interactAction = _playerInput.actions.FindAction("Interact", true);
                if (_interactAction != null)
                {
                    _interactAction.performed += OnInteractPerformed;
                    print("Subscribed to Interact action");
                }
            }
            
            playerUI = other.GetComponent<PlayerInteractionUI>();
            if (playerUI != null)
            {
                playerUI.AddInteractable(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false;
            
            if (craftingUI != null)
            {
                craftingUI.SetActive(false);
            }
            
            //tu się odłącza od akcji w mapie
            UnsubscribeInteract();
            
            if (playerUI != null)
            {
                playerUI.RemoveInteractable(gameObject);
                playerUI = null; 
            }
        }
    }
    
    private void UnsubscribeInteract()
    {
        if (_interactAction != null)
        {
            _interactAction.performed -= OnInteractPerformed;
            _interactAction = null;
        }
        _playerInput = null;
    }
    
    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        //tu poszło to co było w update, logika działania identyczna, o ile nic nie przeoczyłem
        // PauseManager wyłącza mapę, więc callbacki nie będą wywoływane w pauzie
        if (!isPlayerInRange) return;

        if (craftingUI != null)
        {
            craftingUI.SetActive(!craftingUI.activeSelf);
        }
    }
}