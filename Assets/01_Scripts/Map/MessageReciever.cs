using System;
using PaperFlower.Core;
using PaperFlower.Events;
using UnityEngine;

namespace Code.Map
{
    public abstract class MessageReciever : MonoBehaviour
    {
        [SerializeField] protected string message;
        protected virtual void Awake()
        {
            GameEventBus.AddListener<SendMessageEvent>(HandleRecieveMessage);
        }

        protected virtual void OnDestroy()
        {
            GameEventBus.RemoveListener<SendMessageEvent>(HandleRecieveMessage);
        }

        private void HandleRecieveMessage(SendMessageEvent evt)
        {
            if (evt.itemName == message)
                OnMessageRecieved();
        }
        
        protected abstract void OnMessageRecieved();
    }
}