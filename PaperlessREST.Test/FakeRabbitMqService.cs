using System;

namespace PaperlessREST.Test
{
    public class FakeRabbitMqService : IRabbitMqService
    {
        public void Publish(string queue, string message)
        {
            // no implementation needed here for integration tests
        }

        public void SendMessage(string message)
        {
            // no implementation needed here for integration tests
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
