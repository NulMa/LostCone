using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaperFlower.Core
{
    public abstract class GameEvent
    { }
    public static class GameEventBus
    {
        private static Dictionary<Type, Action<GameEvent>> _events = new Dictionary<Type, Action<GameEvent>>();
        private static Dictionary<Delegate, Action<GameEvent>> _lookUpTable = new Dictionary<Delegate, Action<GameEvent>>();

        public static void AddListener<T>(Action<T> handler) where T : GameEvent
        {
            if (_lookUpTable.ContainsKey(handler) == true)
            {
                Debug.Log($"{typeof(T)} is already registered");
                return;
            }
            
            Action<GameEvent> castHandler = evt => handler.Invoke(evt as T);
            _lookUpTable[handler] = castHandler;
            
            Type evtType = typeof(T);
            if (_events.ContainsKey(evtType))
                _events[evtType] += castHandler;
            else
                _events[evtType] = castHandler;
        }

        public static void RemoveListener<T>(Action<T> handler) where T : GameEvent
        {
            Type evtType = typeof(T);
            if (_lookUpTable.TryGetValue(handler, out Action<GameEvent> castHandler))
            {
                if (_events.TryGetValue(evtType, out Action<GameEvent> internalHandler))
                {
                    internalHandler -= castHandler;
                    if (internalHandler == null)
                        _events.Remove(evtType);
                    else
                        _events[evtType] = internalHandler;
                }
                _lookUpTable.Remove(handler);
            }
        }

        public static void RaiseEvent(GameEvent evt)
        {
            if(_events.TryGetValue(evt.GetType(), out Action<GameEvent> castHandler))
            {
                castHandler?.Invoke(evt);
            }
        }

        public static void Clear()
        {
            _events.Clear();
            _lookUpTable.Clear();
        }
    }
}