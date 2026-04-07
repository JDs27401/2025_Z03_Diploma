using C__Classes.Singletons;
using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerVisualSystem : SingletonNonPersistant<PlayerVisualSystem>
    {
        private PlayerController _playerController;
        
        [SerializeField]
        private CircleCollider2D visualTrigger;
        
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
            if (visualTrigger == null)
            {
                print("Visual trigger is not set up in the Editor");
                return;
            }
            visualTrigger.radius = triggerSize;
        }

        public void Update()
        {
            
        }

        public void UpdateVisualRange(bool b)
        {
            switch (b)
            {
                case true:
                    visualTrigger.radius = triggerSize / 2;
                    break;
                case false:
                    visualTrigger.radius = triggerSize;
                    break;
            }
        }
    }
}