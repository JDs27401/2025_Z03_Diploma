using UnityEngine;

namespace C__Classes.Systems
{
    public class LockerAudio : MonoBehaviour
    {
        [Header("Locker Interactable")]
        [SerializeField] private LockerInteractable _lockerInteractable;   
        
        [Header("Audio Source")]
        [SerializeField] private AudioSource _audioSource;
        
        [Header("Sound clip")]
        [SerializeField] private AudioClip _chestOpeningSound;
        [SerializeField] private AudioClip _chestClosingSound;
        
        private void OnEnable()
        {
            if (_lockerInteractable != null)
            {
                _lockerInteractable.OnChestOpen += PlayChestOpenSound;
                _lockerInteractable.OnChestClosed += PlayChestClosedSound;
            }
                
        }

        private void OnDisable()
        {
            if (_lockerInteractable != null)
            {
                _lockerInteractable.OnChestOpen -= PlayChestOpenSound;
                _lockerInteractable.OnChestClosed += PlayChestClosedSound;
            }
        }

        private void PlayChestOpenSound()
        {
            if(_audioSource == null || _chestOpeningSound == null) return;
            
            _audioSource.pitch = 1;
            _audioSource.volume = 0.6f;
            _audioSource.PlayOneShot(_chestOpeningSound);
        }
        
        private void PlayChestClosedSound()
        {
            if(_audioSource == null || _chestClosingSound == null) return;
            
            _audioSource.pitch = 1;
            _audioSource.volume = 0.3f;
            _audioSource.PlayOneShot(_chestClosingSound);
        }
    }
}