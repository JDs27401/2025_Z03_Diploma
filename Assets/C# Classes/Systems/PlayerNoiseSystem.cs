using UnityEngine;

namespace C__Classes.Systems
{
    public class PlayerNoiseSystem : MonoBehaviour
    {
        private PlayerController _playerController;
        [SerializeField] private CircleCollider2D noiseTrigger;
        
        //temporary only, until proper Item data and inventory is implemented
        [SerializeField] private float triggerSize;

        private void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            noiseTrigger.radius = triggerSize;
        }

        private void Update()
        {
            UpdateNoiseRadius(triggerSize);
        }

        private void UpdateNoiseRadius(float newRadius)
        {
            noiseTrigger.radius = newRadius;
        }
    }
}