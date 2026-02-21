using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

namespace Code.Map
{
    public class NekoSteal : MonoBehaviour
    {
        [SerializeField] private SoundSO sound;
        [SerializeField] private bool canPlay;
        private readonly PlaySFXEvent _playSFXEvent = new PlaySFXEvent();

        public void PlaySound()
        {
            if (canPlay == false) return;
            GameEventBus.RaiseEvent(_playSFXEvent.Initialize(sound, gameObject.transform.position));
        }
    }
}