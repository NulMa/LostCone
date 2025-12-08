using System.IO;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public struct BoolData
    {
        public bool Actived { get; }

        public BoolData(bool actived)
        {
            Actived = actived;
        }
    }
    
    [System.Serializable]
    public class PositionData
    {
        // public Vector3 Position { get; set; } <-- 제거
    
        // 👇 public float 필드 3개로 대체
        public float x;
        public float y;
        public float z;

        // 생성자는 Vector3를 받아 세 개의 float에 값을 할당하도록 수정
        public PositionData(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }
    }


    public class SaveManager : MonoSingleton<SaveManager>
    {
        private string GetPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key + ".json");
        }

        public void Save<T>(T data, string saveKey)
        {
            string path = GetPath(saveKey);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        public T Load<T>(string saveKey) where T : class
        {
            string path = GetPath(saveKey);

            if (!File.Exists(path))
            {
                Debug.LogError($"{path} not found");
                return null;
            }

            Debug.Log($"Load{saveKey}");
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }


        [ContextMenu("Clear Data")]
        public void ClearData(string saveKey)
        {
            string path = GetPath(saveKey);
            if (File.Exists(path))
                File.Delete(path);
        }

        [ContextMenu("Clear all Data")]
        public void ClearAllData()
        {
            string path = Application.persistentDataPath;
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }
}