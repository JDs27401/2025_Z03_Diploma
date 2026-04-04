using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerVisualSystem : MonoBehaviour
    {
        public static PlayerVisualSystem Instance;
        
        private PlayerController _playerController;
        
        [SerializeField]
        private CircleCollider2D visualTrigger;
        
        //temporary only, until proper Item data and inventory is implemented
        [SerializeField] private float triggerSize;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            if (_playerController == null)
            {
                print("Player Controller not found");
                return;
            }
            if (visualTrigger == null)
            {
                print("Noise trigger is not set up in the Editor");
                return;
            }
            visualTrigger.radius = triggerSize;
        }

        public void Update()
        {
            
        }

        public void UpdateVisualRange(bool b)
        {
            
        }
    }
}