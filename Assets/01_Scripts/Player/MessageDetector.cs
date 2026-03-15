using System;
using PaperFlower.Core;
using PaperFlower.Events;
using UnityEngine;

namespace Code.Tongary
{
    public class MessageDetector : MonoBehaviour
    {
        private readonly SendMessageEvent _messageEvent = PlayerEvents.SendMessageEvent;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out MessageSender sender))
            {
                GameEventBus.RaiseEvent(_messageEvent.Init(sender.Message));
            }
        }
    }
}