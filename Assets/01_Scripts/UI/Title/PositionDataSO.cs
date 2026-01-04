using UnityEngine;

namespace _01_Scripts.UI.Title
{
    [CreateAssetMenu(fileName = "PositionData", menuName = "SO/PositionData", order = 0)]
    public class PositionDataSO : ScriptableObject
    {
        public Vector2 position;
    }
}