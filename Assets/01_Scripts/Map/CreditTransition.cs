using System;
using UnityEngine;

namespace Code.Map
{
    public class CreditTransition : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credit");
            }
        }
    }
}