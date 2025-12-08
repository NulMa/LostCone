using UnityEngine;

namespace Core
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object locker = new();
        private static T instance;
    
        public static T Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = (T)FindAnyObjectByType(typeof(T));
                        
                        if (instance == null)
                        {
                            var temp = new GameObject(name: $"@{typeof(T)}");
                            instance = temp.AddComponent<T>();
                        }
                    }
                }
                
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}