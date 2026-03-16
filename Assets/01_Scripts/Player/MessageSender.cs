using UnityEngine;

namespace Code.Tongary
{
    public class MessageSender : MonoBehaviour
    {
        [field: SerializeField] public string Message { get; private set; }
    }
}