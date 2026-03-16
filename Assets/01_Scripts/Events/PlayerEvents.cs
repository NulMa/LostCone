using PaperFlower.Core;

namespace PaperFlower.Events
{
    public static class PlayerEvents
    {
        public static SendMessageEvent SendMessageEvent = new();
    }

    public class SendMessageEvent : GameEvent
    {
        public string itemName;

        public SendMessageEvent Init(string itemName)
        {
            this.itemName = itemName;
            return this;
        }
    }
}