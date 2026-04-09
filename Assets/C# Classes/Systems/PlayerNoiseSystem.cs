using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerNoiseSystem : MonoBehaviour
    {
        public static PlayerNoiseSystem Instance { get; private set; }
        
        private PlayerController _playerController;
        [SerializeField] private CircleCollider2D noiseTrigger;
        
        [Header("Konfiguracja Hałasu")]
        [SerializeField] private float baseTriggerSize = 5f;
        [SerializeField] private float sprintMultiplier = 1.5f;
        
        [Header("Wpływ Wagi na Hałas")]
        [SerializeField] private float weightNoiseMultiplier = 0.01f; // Noise penalty per 1 point (ex. 0.01 = +1% noise for 1 kg)

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
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
                Debug.LogWarning("[PlayerNoiseSystem] Player Controller not found");
                return;
            }
            if (noiseTrigger == null)
            {
                Debug.LogWarning("[PlayerNoiseSystem] Noise trigger is not set up in the Editor");
                return;
            }
            
            UpdateNoiseRadius();
        }

        public void UpdateNoiseRadius()
        {
            if (noiseTrigger == null || _playerController == null) return;

            float calculatedRadius = baseTriggerSize;

            if (_playerController.IsSprinting())
            {
                calculatedRadius *= sprintMultiplier;
            }

            if (global::InventoryManager.instance != null)
            {
                float currentWeight = global::InventoryManager.instance.GetTotalWeight();
                
                float weightFactor = 1f + (currentWeight * weightNoiseMultiplier);
                
                calculatedRadius *= weightFactor;
            }

            noiseTrigger.radius = calculatedRadius;
        }
    }
}