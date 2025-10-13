namespace PaperlessREST.Application
{
    public interface IMessageQueue
    {
        void SendMessage(string message);
    }
}
