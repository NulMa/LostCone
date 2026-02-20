using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

namespace Code.Player
{
    public class PlayerFootstep : MonoBehaviour
    {
        public bool canPlay;
        
        [SerializeField] private SoundSO[] stepSounds;
        private readonly PlaySFXEvent _playSFXEvent = new PlaySFXEvent();

        public void PlayFootstep()
        {
            if (stepSounds.Length == 0 || !canPlay) return;
            var sound = stepSounds[Random.Range(0, stepSounds.Length)];
            GameEventBus.RaiseEvent(_playSFXEvent.Initialize(sound));
        }
    }
}