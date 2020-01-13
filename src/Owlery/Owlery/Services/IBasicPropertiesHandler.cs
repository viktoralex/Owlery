using Owlery.Models;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public interface IBasicPropertiesHandler
    {
         IBasicProperties ApplyMessageProperties(RabbitMessage message, IBasicProperties properties);
    }
}