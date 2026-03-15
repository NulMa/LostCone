using UnityEngine;
using Core;

namespace KimMin.ObjectPool.RunTime
{
    public class PoolManagerMono : MonoSingleton<PoolManagerMono>
    {
        [SerializeField] private PoolManagerSO poolManager;

        protected override void Awake()
        {
            base.Awake();
            poolManager.Initialize(transform);
        }
        
        public T Pop<T>(PoolItemSO item) where T : IPoolable
        {
            return (T)poolManager.Pop(item);
        }
        
        public void Push(IPoolable item)
        {
            poolManager.Push(item);
        }
    }
}