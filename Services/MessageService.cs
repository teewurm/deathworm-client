namespace DeathWorm.Services
{
    public record Message(DateTime Timestamp, string Text);

    public class MessageService
    {
        private readonly List<Message> _messages = new();
        private readonly object _messagesLock = new();
        private const int MaxMessages = 10;

        public void AddMessage(string text)
        {
            lock (_messagesLock)
            {
                _messages.Add(new Message(DateTime.Now, text));
                if (_messages.Count > MaxMessages)
                    _messages.RemoveAt(0);
            }
        }

        public List<Message> GetMessages()
        {
            lock (_messagesLock)
            {
                return _messages.ToList();
            }
        }
    }
}
