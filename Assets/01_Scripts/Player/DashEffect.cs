using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

namespace Code.Player
{
    public class DashEffect : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private SoundSO dashSound;
        private readonly PlaySFXEvent _playSFXEvent = new PlaySFXEvent();

        private float _dashCooltime = 0.5f;

        public void PlayEffect()
        {
            animator?.SetTrigger("OnDash");
            GameEventBus.RaiseEvent(_playSFXEvent.Initialize(dashSound));
        }

        public void SetFlip(bool isFlipX)
        {
            renderer.flipX = isFlipX;
        }
    }
}