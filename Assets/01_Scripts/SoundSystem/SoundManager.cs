using System.Collections.Generic;
using Blade.SoundSystem;
using Core;
using KimMin.ObjectPool.RunTime;
using PaperFlower.Core;
using UnityEngine;

namespace Blade.Managers
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private PoolItemSO soundPlayer; 
        
        private Dictionary<int, SoundPlayer> _soundPlayerDict = new();

        protected override void Awake()
        {
            base.Awake();
            GameEventBus.AddListener<PlaySoundEvent>(HandlePlaySFXEvent);
            GameEventBus.AddListener<StopSoundEvent>(HandleStopSoundEvent);
            StopAllLoopSounds();
            
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (_, __) =>
            {
                StopAllLoopSounds();
            };
        }

        private void OnDestroy()
        {
            GameEventBus.RemoveListener<PlaySoundEvent>(HandlePlaySFXEvent);
            GameEventBus.RemoveListener<StopSoundEvent>(HandleStopSoundEvent);
        }

        private void HandlePlaySFXEvent(PlaySoundEvent evt)
        {
            SoundPlayer player = PoolManagerMono.Instance.Pop<SoundPlayer>(soundPlayer);
            player.PlaySound(evt.clip);

            if (evt.withPosition)
            {
                player.SetPosition(evt.position);
            }

            if (evt.channel > 0 && evt.clip.loop)
            {
                if (_soundPlayerDict.TryGetValue(evt.channel, out SoundPlayer beforePlayer))
                {
                    beforePlayer.StopAndGotoPool();
                    _soundPlayerDict.Remove(evt.channel);
                }
                _soundPlayerDict.Add(evt.channel, player);
            }
            else if(evt.channel <= 0 && evt.clip.loop)
            {
                Debug.LogWarning($"사운드 루프 설정이 되었으나 채널이 0 이하입니다. {evt.clip.name}");   
            }
        }

        private void HandleStopSoundEvent(StopSoundEvent evt)
        {
            if (_soundPlayerDict.TryGetValue(evt.channel, out SoundPlayer beforePlayer))
            {
                beforePlayer.StopAndGotoPool();
                _soundPlayerDict.Remove(evt.channel);
            }
        }
        
        public void StopAllLoopSounds()
        {
            foreach (var pair in _soundPlayerDict)
            {
                pair.Value.StopAndGotoPool();
            }
            _soundPlayerDict.Clear();
        }
    }
}