using C__Classes;
using C__Classes.Pipelines;
using C__Classes.Systems;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Player.scripts
{
    using UnityEngine;
    
    public class PlayerAudio : MonoBehaviour
    {
        private WeaponController _weaponController;
        private Actor _actor;
        private PlayerController _playerController;
        
        [FormerlySerializedAs("Weapon Audio Source")]
        [Header("Audio Source")] 
        [SerializeField] public AudioSource weaponAudioSource;
        
        [Header("MovementAudioSource")]
        [SerializeField] public AudioSource movementAudioSource;
        
        
        [Header("Sound clipy broni")]
        [SerializeField] public AudioClip pistolSound;
        [SerializeField] public AudioClip shotgunSound;
        
        [Header("Sound clipy movementu")]
        [SerializeField] public AudioClip groundWalkingSound;
        [SerializeField] public AudioClip groundRunningSound;
        [SerializeField] public AudioClip groundDashSound;
        [SerializeField] public AudioClip swimmingSound;
        [SerializeField] public AudioClip gettingHurtSound;
        [SerializeField] public AudioClip deathSound;

        private void Awake()
        {
            if (_weaponController == null)
            {
                _weaponController = gameObject.AddComponent<WeaponController>();
            }
                
            
            if (_actor == null)
                _actor = gameObject.GetComponent<Actor>();
            
            
            if (_playerController == null)
                _playerController = gameObject.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (_playerController == null || !AreAudioWeaClipsFilledMovement()) return;

            if (_playerController.IsMoving)
            {
                PlayWalkingSound();
            }
            else
            {
                if(movementAudioSource.isPlaying)
                    movementAudioSource.Stop();
            }

        }
        
        private void OnEnable()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponFired += PlayWeaponSound;

            if (_playerController != null)
            {
                _playerController.OnPlayerRoll += PlayRollSound;
                _playerController.OnPlayerHurt += PlayHurtSound;
                _playerController.OnPlayerDeathAudio += PlayDeathSound;
            }
        }

        private void OnDisable()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponFired -= PlayWeaponSound;

            if (_playerController != null)
            {
                _playerController.OnPlayerRoll -= PlayRollSound;
                _playerController.OnPlayerHurt -= PlayHurtSound;
                _playerController.OnPlayerDeathAudio -= PlayDeathSound;
            }
        }


        private void PlayWeaponSound()
        {
            if (AreAudioWeaClipsFilledWeapons())
            {
                weaponAudioSource.pitch = Random.Range(0.9f, 1.1f);
                switch (GetCurrentWeapon().ToLower())
                {
                    case "pistol":
                        weaponAudioSource.PlayOneShot(pistolSound);
                        break;
                    case "shotgun":
                        weaponAudioSource.PlayOneShot(shotgunSound);
                        break;
                }
            }
        }

        private void PlayWalkingSound()
        {
            AudioClip targetClip = null;
            
            bool isSprinting = _playerController.IsSprinting();
            
            switch (GetCurrentTileType())
            { 
                case TileType.Ground: 
                    targetClip = isSprinting ? groundRunningSound : groundWalkingSound;
                    break;
                case TileType.Water:
                    targetClip = swimmingSound;
                    break;
            }

            if (targetClip == null) return;

            if (movementAudioSource.clip != targetClip)
            {
                movementAudioSource.clip = targetClip;
                movementAudioSource.pitch = Random.Range(0.9f, 1.1f);
                movementAudioSource.Play();    
            }
            else if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.pitch = Random.Range(0.9f, 1.1f);
                movementAudioSource.Play();    
            }
        }

        private string GetCurrentWeapon()
        {
            return _weaponController.currentWeapon.weaponName;
        }

        private TileType GetCurrentTileType()
        {
            return _actor.TileType;
        }

        private void PlayRollSound()
        {
            if (weaponAudioSource != null && groundDashSound != null)
            {
                weaponAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                weaponAudioSource.PlayOneShot(groundDashSound);
            }
        }

        private void PlayHurtSound()
        {
            if (weaponAudioSource != null && gettingHurtSound != null)
            {
                weaponAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                weaponAudioSource.PlayOneShot(gettingHurtSound);
            }
        }

        private void PlayDeathSound()
        {
            GameObject audioObject = new GameObject("player_death_sound_temp");
            audioObject.transform.position = transform.position;
            
            AudioSource tempSource = audioObject.AddComponent<AudioSource>();
            
            tempSource.clip = deathSound;
            tempSource.pitch = Random.Range(0.9f, 1.1f);
            tempSource.volume = 0.5f;
            
                tempSource.Play();
            
            Destroy(audioObject, tempSource.clip.length);
        }

        private bool AreAudioWeaClipsFilledWeapons()
        {
            return weaponAudioSource is not null && pistolSound is not null && shotgunSound is not null;
        }
        
        private bool AreAudioWeaClipsFilledMovement()
        {
            return movementAudioSource is not null && groundWalkingSound is not null && swimmingSound is not null;
        }
    }
}