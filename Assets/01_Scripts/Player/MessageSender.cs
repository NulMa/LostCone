using UnityEngine;

namespace Code.Player
{
    public class MessageSender : MonoBehaviour
    {
        [field: SerializeField] public string Message { get; private set; }
    }
}