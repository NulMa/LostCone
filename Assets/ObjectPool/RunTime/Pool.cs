using System.Collections.Generic;
using UnityEngine;

namespace KimMin.ObjectPool.RunTime
{
    public class Pool
    {
        private readonly Stack<IPoolable> _pool;
        private readonly Transform _parentTrm;
        private readonly GameObject _prefab;

        public Pool(IPoolable poolable, Transform parentTrm, int count)
        {
            _pool = new Stack<IPoolable>(count);
            _parentTrm = parentTrm;
            _prefab = poolable.GameObject;

            for (int i = 0; i < count; i++)
            {
                GameObject gameObj = GameObject.Instantiate(_prefab,_parentTrm);
                gameObj.SetActive(false);
                IPoolable item = gameObj.GetComponent<IPoolable>();
                item.SetUpPool(this);
                _pool.Push(item);
            }
        }

        public IPoolable Pop()
        {
            IPoolable item = null;

            // 1. 유효한 아이템이 나올 때까지 스택을 뒤집니다.
            while (_pool.Count > 0)
            {
                var potentialItem = _pool.Pop();
        
                // 2. 파괴되었는지 확인 (Unity의 Null 체크 방식)
                if (potentialItem != null && !potentialItem.Equals(null)) 
                {
                    item = potentialItem;
                    break; // 찾았으면 탈출
                }
            }

            // 3. 만약 유효한 아이템이 없거나 스택이 비어있다면 새로 생성
            if (item == null)
            {
                GameObject gameObj = GameObject.Instantiate(_prefab, _parentTrm);
                item = gameObj.GetComponent<IPoolable>();
                item.SetUpPool(this);
            }
            else
            {
                item.GameObject.SetActive(true);
            }

            item.ResetItem();
            return item;
        }

        public void Push(IPoolable item)
        {
            item.GameObject.SetActive(false);
            _pool.Push(item);
        }
    }
}