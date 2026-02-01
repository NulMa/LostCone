using UnityEngine;

namespace Code.Player
{
    public class DashEffect : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer renderer;

        private float _dashCooltime = 0.5f;

        public void PlayEffect()
        {
            animator?.SetTrigger("OnDash");
            Debug.Log("Dash");
        }

        public void SetFlip(bool isFlipX)
        {
            renderer.flipX = isFlipX;
        }
    }
}