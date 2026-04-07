using C__Classes.Singletons;
using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerNoiseSystem : SingletonNonPersistant<PlayerNoiseSystem>
    {
        private PlayerController _playerController;
        [SerializeField] private CircleCollider2D noiseTrigger;
        
        //temporary only, until proper Item data and inventory is implemented
        [SerializeField] private float triggerSize;
        
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
            // UpdateNoiseRadius(triggerSize);
        }

        public void UpdateNoiseRadius()
        {
            switch (_playerController.IsSprinting())
            {
                case true:
                    noiseTrigger.radius *= 1.5f;
                    break;
                case false:
                    noiseTrigger.radius = triggerSize;
                    break;
            }
            //@todo rest of the math involved related to equipment weight, or a completely different function for calculating it
        }
        
        // public void UpdateNoiseRadius(float newRadius)
        // {
        //     noiseTrigger.radius = newRadius;
        // }
    }
}