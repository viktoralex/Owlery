using Owlery.Models;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public interface IRabbitService
    {
         void Publish(object body, string routingKey, string exchange = null);
         void Publish(RabbitMessage message, string routingKey, string exchange = null);
    }
}