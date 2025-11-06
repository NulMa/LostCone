using System;
using PaperFlower.Core;
using PaperFlower.Events;
using UnityEngine;

namespace Code.Player
{
    public class MessageDetector : MonoBehaviour
    {
        private readonly SendMessageEvent _messageEvent = PlayerEvents.SendMessageEvent;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.name);
            if (other.TryGetComponent(out MessageSender sender))
            {
                Debug.Log("SDF");
                GameEventBus.RaiseEvent(_messageEvent.Init(sender.Message));
            }
        }
    }
}