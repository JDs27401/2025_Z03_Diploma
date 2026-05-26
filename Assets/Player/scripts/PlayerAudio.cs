using C__Classes;
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
        [SerializeField] public AudioClip swimmingSound;

        private void Awake()
        {
            if (_weaponController == null)
                _weaponController = gameObject.AddComponent<WeaponController>();
            
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
            {
                _weaponController.OnWeaponFired += PlayWeaponSound;
            }
        }

        private void OnDisable()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponFired -= PlayWeaponSound;
        }


        private void PlayWeaponSound()
        {
            if (AreAudioWeaClipsFilledWeapons())
            {
                weaponAudioSource.pitch = Random.Range(0.9f, 1.1f);
                switch (GetCurrentWeapon().ToLower())
                {
                    case "pistol":
                        break;
                    case "shotgun":
                        break;
                }
            }
        }

        private void PlayWalkingSound()
        {
            movementAudioSource.pitch = Random.Range(0.9f, 1.1f);
            
            if (movementAudioSource.isPlaying) return;
            
            switch (GetCurrentTileType())
            { 
                case TileType.Ground: 
                    movementAudioSource.clip = groundWalkingSound;
                    movementAudioSource.Play();
                    break;
                case TileType.Water:
                    movementAudioSource.clip = swimmingSound;
                    movementAudioSource.Play();
                    break;
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