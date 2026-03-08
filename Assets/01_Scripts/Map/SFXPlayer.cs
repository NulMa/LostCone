using System;
using System.Collections;
using System.Collections.Generic;
using Blade.SoundSystem;
using PaperFlower.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Map
{
    public class SFXPlayer : MonoBehaviour
    {
        [SerializeField] private SoundSO sound;
        [SerializeField] private bool isPlayOnAwake = false;
        [SerializeField] private float delay = 0f;
        [SerializeField] private bool isSpread = true;
        
        private PlaySoundEvent _playSoundEvent = new PlaySoundEvent();

        private void Awake()
        {
            if (isPlayOnAwake)
            {
                PlaySFX();
            }
        }

        public void PlaySFX()
        {
            StartCoroutine(PlaySFXRoutine());
        }

        private IEnumerator PlaySFXRoutine()
        {
            yield return new WaitForSeconds(delay);

            int id = sound.loop ? 1 : 0;
            if (isSpread)
                GameEventBus.RaiseEvent(_playSoundEvent.Initialize(sound, transform.position, id));
            else
                GameEventBus.RaiseEvent(_playSoundEvent.Initialize(sound, id));
        }
    }
}