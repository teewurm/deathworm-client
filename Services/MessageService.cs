namespace DeathWorm.Services
{
    public class MessageService
    {
        private readonly List<string> _messages = new();
        private readonly object _messagesLock = new();
        private const int MaxMessages = 10;

        public void AddMessage(string message)
        {
            lock (_messagesLock)
            {
                _messages.Add(message);
                if (_messages.Count > MaxMessages)
                    _messages.RemoveAt(0);
            }
        }

        public List<string> GetMessages()
        {
            lock (_messagesLock)
            {
                return _messages.ToList();
            }
        }
    }
}
