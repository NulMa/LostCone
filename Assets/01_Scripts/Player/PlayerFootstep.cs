using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

namespace Code.Player
{
    public class PlayerFootstep : MonoBehaviour
    {
        public bool canPlay;
        public bool isPosition;
        
        [SerializeField] private SoundSO[] stepSounds;
        private readonly PlaySoundEvent _playSoundEvent = new PlaySoundEvent();

        public void PlayFootstep()
        {
            if (stepSounds.Length == 0 || !canPlay) return;
            var sound = stepSounds[Random.Range(0, stepSounds.Length)];
            if (isPosition)
                GameEventBus.RaiseEvent(_playSoundEvent.Initialize(sound, gameObject.transform.position));
            else
                GameEventBus.RaiseEvent(_playSoundEvent.Initialize(sound));
        }
    }
}