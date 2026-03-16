using System;
using UnityEngine;

namespace Code.Map
{
    public class AirFan : MonoBehaviour
    {
        [SerializeField] private float power;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                player.rigid.AddForce(Vector2.up * power * Time.deltaTime, ForceMode2D.Impulse);
            }
        }
    }
}