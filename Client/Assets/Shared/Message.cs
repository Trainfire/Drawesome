namespace Protocol
{
    public class Message
    {
        public string LogMessage;
        public MessageType Type;

        public Message()
        {

        }

        public Message(MessageType type, string logMessage = "")
        {
            LogMessage = logMessage;
            Type = type;
        }

        public T Deserialise<T>(string json)
        {
            return JsonHelper.FromJson<T>(json);
        }

        public string AsJson()
        {
            return JsonHelper.ToJson(this);
        }
    }
}
