using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Code.Map
{
    public class CreditTransition : MonoBehaviour
    {

        [SerializeField] GameObject panel;
        private async Task OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                panel.SetActive(true);
                await Awaitable.WaitForSecondsAsync(2);

                UnityEngine.SceneManagement.SceneManager.LoadScene("Credit");
            }
        }
    }
}