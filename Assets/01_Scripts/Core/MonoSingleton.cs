using UnityEngine;

namespace Core
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
    
        public static T Instance
        {
            get
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

                return instance;
            }
        }

        protected virtual void Awake()
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