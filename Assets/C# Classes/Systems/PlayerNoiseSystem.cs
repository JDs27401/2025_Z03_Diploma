using C__Classes.Managers;
using C__Classes.Singletons;
using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerNoiseSystem : SingletonNonPersistant<PlayerNoiseSystem>
    {
        private PlayerController _playerController;
        [SerializeField] private CircleCollider2D noiseTrigger;
        
        [Header("Noise configuration")]
        [SerializeField] private float baseTriggerSize = 5f;
        [SerializeField] private float sprintMultiplier = 1.5f;
        [SerializeField] private float weightNoiseMultiplier = 0.01f; // Noise penalty per 1 point (ex. 0.01 = +1% noise for 1 kg)

        
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

            if (InventoryManager.Instance != null)
            {
                float currentWeight = InventoryManager.Instance.GetTotalWeight();
                
                float weightFactor = 1f + (currentWeight * weightNoiseMultiplier);
                
                calculatedRadius *= weightFactor;
            }

            // Apply consumable noise multiplier if available (prefer cached from player)
            float noiseMultiplier = 1f;
            if (_playerController != null)
            {
                noiseMultiplier = _playerController.GetCachedNoiseMultiplier();
            }
            else if (InventoryManager.Instance != null)
            {
                noiseMultiplier = InventoryManager.Instance.GetConsumableNoiseMultiplier();
            }

            calculatedRadius *= noiseMultiplier;

            noiseTrigger.radius = calculatedRadius;
        }
    }
}