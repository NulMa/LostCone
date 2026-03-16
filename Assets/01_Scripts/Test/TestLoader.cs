using System;
using UnityEngine;

namespace _01_Scripts.Test
{
    public class TestLoader : MonoBehaviour
    {
        private void Start()
        {
            if (GamaManager.Instance != null)
            {
                GamaManager.Instance.LoadGame();
            }
        }
    }
}