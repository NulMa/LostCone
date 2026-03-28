using KimMin.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Blade.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup bgmGroup;
        
        private AudioSource _audioSource;
        private Pool _myPool;
        
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        public GameObject GameObject => gameObject;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _myPool.Push(this);
        }
        
        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void SetUpPool(Pool pool) => _myPool = pool;

        public void ResetItem()
        { }

        public void PlaySound(SoundSO data)
        {
            _audioSource.spatialBlend = 0f;

            _audioSource.outputAudioMixerGroup = data.audioType switch
            {
                SoundSO.AudioTypes.SFX => sfxGroup,
                SoundSO.AudioTypes.MUSIC => bgmGroup,
                _ => sfxGroup
            };
            
            _audioSource.volume = data.volume;
            _audioSource.pitch = data.pitch;

            if (data.randomizePitch)
            {
                _audioSource.pitch += Random.Range(-data.randomPitchModifier, data.randomPitchModifier);
            }
            _audioSource.clip = data.clip;
            _audioSource.loop = data.loop;

            if (!data.loop)
            {
                float duration = _audioSource.clip.length + 0.2f;
                DisableSound(duration);
            }

            _audioSource.Play();
        }

        public void SetPosition(Vector3 position)
        {
            _audioSource.spatialBlend = 1.0f;
            transform.position = position;
        }

        private async void DisableSound(float duration)
        {
            await Awaitable.WaitForSecondsAsync(duration);
            _myPool.Push(this);
        }

        public void StopAndGotoPool()
        {
            _audioSource.Stop();
            _myPool.Push(this);
        }
    }
}