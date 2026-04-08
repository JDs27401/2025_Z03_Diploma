using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerNoiseSystem : MonoBehaviour
    {
        public static PlayerNoiseSystem Instance { get; private set; }
        
        private PlayerController _playerController;
        [SerializeField] private CircleCollider2D noiseTrigger;
        
        [Header("Konfiguracja Hałasu")]
        [SerializeField] private float baseTriggerSize = 5f; // Bazowy promień hałasu
        [SerializeField] private float sprintMultiplier = 1.5f; // Mnożnik podczas sprintu
        
        [Header("Wpływ Wagi na Hałas")]
        [SerializeField] private float weightNoiseMultiplier = 0.01f; // Kara do hałasu za 1 jednostkę wagi (np. 0.01 = +1% hałasu za 1 kg)

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
                
                // Brak limitu wagi - każdy punkt wagi poszerza promień
                float weightFactor = 1f + (currentWeight * weightNoiseMultiplier);
                
                calculatedRadius *= weightFactor;
            }

            noiseTrigger.radius = calculatedRadius;
        }
    }
}