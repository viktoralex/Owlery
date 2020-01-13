using Owlery.Models;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public interface IBasicPropertiesHandler
    {
        void ApplySettingsProperties(IBasicProperties properties);
        void ApplyMessageProperties(RabbitMessage message, IBasicProperties properties);
    }
}