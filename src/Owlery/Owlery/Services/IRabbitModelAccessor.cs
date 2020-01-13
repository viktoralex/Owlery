using RabbitMQ.Client;

namespace Owlery.Services
{
    public interface IRabbitModelAccessor
    {
         IModel GetModel();
    }
}