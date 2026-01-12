using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRabbitMqService : IDisposable
{
    void Publish(string queue, string message);
    void SendMessage(string message);
}