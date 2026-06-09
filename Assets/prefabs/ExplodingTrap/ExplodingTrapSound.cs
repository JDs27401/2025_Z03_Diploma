using C__Classes.Objects;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplodingTrapSound : MonoBehaviour
{
     
        
        [Header("Skrypt od komponentu do eksplozji")]
        [SerializeField] private ExplodingComponent _explodingComponent;
        
        [Header("Audio Source")]
        [SerializeField] private AudioSource _audioSource;
        
        [FormerlySerializedAs("_basicGrowlSound")]
        [Header("Sound clipy")]
        [SerializeField] private AudioClip _explosionSound;
        

        private void OnEnable()
        {
            if(_explodingComponent == null) return;
            
            _explodingComponent.OnExplosion += PlayExplosionSound;
            }
        
        
        private void OnDisable()
        {
            if(_explodingComponent == null) return;
            
            _explodingComponent.OnExplosion -= PlayExplosionSound;
        }

        private void PlayExplosionSound()
        {
            if (AreAudioSourcesFilled())
            {
                _audioSource.pitch = Random.Range(0.9f, 1.1f);
                _audioSource.volume = 0.2f;
                _audioSource.PlayOneShot(_explosionSound);
            }
                
        }

        private bool AreAudioSourcesFilled()
        {
            return !(_explodingComponent == null && _audioSource == null && _explosionSound == null); 
        }
}
