using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

namespace Code.Map
{
    public class AnimationSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundSO sound;
        [SerializeField] private bool canPlay;
        private readonly PlaySoundEvent _playSoundEvent = new PlaySoundEvent();

        public void PlaySound()
        {
            if (canPlay == false) return;
            GameEventBus.RaiseEvent(_playSoundEvent.Initialize(sound, gameObject.transform.position));
        }
    }
}