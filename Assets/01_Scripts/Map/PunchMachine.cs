using System;
using DG.Tweening;
using UnityEngine;

namespace Code.Map
{
    public class PunchMachine : MonoBehaviour
    {
        [SerializeField] private bool isRight;
        [SerializeField] private float force = 100f;
        [SerializeField] private Vector2 overlapSize;
        [SerializeField] private LayerMask whatIsPlayer;

        public void OnPunch()
        {
            Vector3 dir = isRight ? Vector2.right : Vector2.left;
            var target = Physics2D.OverlapBox(transform.position + dir, overlapSize, 0f, whatIsPlayer);
            if (target == null) return;
            
            if (target.TryGetComponent(out Player player))
            {
                player.ApplyKnockback(dir * force);
            }
        }

        private void OnValidate()
        {
            gameObject.transform.localScale = isRight ? Vector3.one : new Vector3(-1, 1, 1);
        }

        private void OnDrawGizmos()
        {
            Vector3 dir = isRight ? Vector2.right : Vector2.left;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + dir, overlapSize);
        }
    }
}