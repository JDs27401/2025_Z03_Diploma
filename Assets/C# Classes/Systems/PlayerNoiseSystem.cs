using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerNoiseSystem : MonoBehaviour
    {
        public PlayerNoiseSystem Instance { get; private set; }
        
        private PlayerController _playerController;
        [SerializeField] private CircleCollider2D noiseTrigger;
        
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
            if (noiseTrigger == null)
            {
                print("Noise trigger is not set up in the Editor");
                return;
            }
            noiseTrigger.radius = triggerSize;
        }

        private void Update()
        {
            //@todo remove this as soon as we get a proper implementation. Change should only be called from UpdateNoiseRadius on weight changes
            UpdateNoiseRadius(triggerSize);
        }

        public void UpdateNoiseRadius(float newRadius)
        {
            noiseTrigger.radius = newRadius;
        }
    }
}