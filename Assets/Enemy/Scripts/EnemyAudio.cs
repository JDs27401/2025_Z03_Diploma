using JetBrains.Annotations;
using UnityEngine;

namespace Enemy.Scripts
{
    public class EnemyAudio : MonoBehaviour
    {

        private Coroutine _resumeGrowlSoundCoroutine;
        
        [Header("Skrypt od enemy")]
        [SerializeField] private NpcBase _npcBase;
        [SerializeField] [CanBeNull] private ExplodingEnemy _explodingEnemy;
        
        [Header("Audio Source")]
        [SerializeField] private AudioSource _audioSource;
        
        [Header("Sound clipy")]
        [SerializeField] private AudioClip _basicGrowlSound;
        [SerializeField] private AudioClip _gettingHurtSound;
        [SerializeField] private AudioClip _deathSound;
        [SerializeField] [CanBeNull] private AudioClip _deathSoundExplosion;

        private void OnEnable()
        {
            if(_npcBase == null) return;
            
            _npcBase.OnHurt += PlayGettingHurtSound;
            _npcBase.OnDeath += PlayNormalDeathSound;
            _npcBase.OnAggravated += PlayGrowlSound;
            _npcBase.OnSleep += StopGrowlSound;

            if (_explodingEnemy != null)
                _explodingEnemy.OnExplode += PlayExplosionDeathSound;
        }
        
        
        private void OnDisable()
        {
            if(_npcBase == null) return;
            
            _npcBase.OnHurt -= PlayGettingHurtSound;
            _npcBase.OnDeath -= PlayNormalDeathSound;
            _npcBase.OnAggravated -= PlayGrowlSound;
            _npcBase.OnSleep -= StopGrowlSound;
                
            if(_explodingEnemy !=null)
                _explodingEnemy.OnExplode -= PlayExplosionDeathSound;
        }

        private void PlayGrowlSound()
        {
            Debug.Log("PlayGrowlSound");
            if (AreAudioSourcesFilled())
            {
                _audioSource.pitch = Random.Range(0.9f, 1.1f);
                _audioSource.volume = 0.05f;
                _audioSource.clip = _basicGrowlSound;
                _audioSource.loop = true;
                _audioSource.Play();
            }
                
        }

        private void StopGrowlSound()
        {
            if (_resumeGrowlSoundCoroutine != null)
            {
                StopCoroutine(_resumeGrowlSoundCoroutine);
                _resumeGrowlSoundCoroutine = null;
            }
            
            if(!AreAudioSourcesFilled()) return;
            
            _audioSource.Stop();
        }

        private void PlayGettingHurtSound()
        {
            if (!AreAudioSourcesFilled()) return;
            
            if(_audioSource.isPlaying)
                _audioSource.Stop();
            
            if(_resumeGrowlSoundCoroutine != null)
                StopCoroutine(_resumeGrowlSoundCoroutine);
            
            Debug.Log("Dostaje obrazenia!");
            
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.loop = false;
            _audioSource.volume = 0.7f;
            _audioSource.PlayOneShot(_gettingHurtSound);
            
            _resumeGrowlSoundCoroutine = StartCoroutine(ResumeGrowlRoutine(_gettingHurtSound.length));
        }
        
        
        private void PlayDeathSound(AudioClip clip, float volume, float delay = 0f)
        {
            if (!AreAudioSourcesFilled()) return;

            GameObject audioObject = new GameObject("death_sound_temp");
            audioObject.transform.position = transform.position;
            
            AudioSource tempSource = audioObject.AddComponent<AudioSource>();
            
            tempSource.clip = clip;
            tempSource.pitch = Random.Range(0.9f, 1.1f);
            tempSource.volume = volume;
            
            if (delay > 0f)
                tempSource.PlayDelayed(delay);
            else
                tempSource.Play();
            
            Destroy(audioObject, tempSource.clip.length + delay);
        }

        private void PlayExplosionDeathSound()
        {
            StopGrowlSound();
            PlayDeathSound(_deathSoundExplosion, 0.2f, 1.5f);
        }

        private void PlayNormalDeathSound()
        {
            StopGrowlSound();
            PlayDeathSound(_deathSound, 0.5f);
        }

        private bool AreAudioSourcesFilled()
        {
            return _audioSource is not null && _basicGrowlSound is not null && _gettingHurtSound is not null && _deathSound is not null;
        }

        private System.Collections.IEnumerator ResumeGrowlRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            PlayGrowlSound();
        }
        
    }
}