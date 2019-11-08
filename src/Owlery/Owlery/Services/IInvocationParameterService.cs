using Owlery.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Owlery.Services
{
    public interface IInvocationParameterService
    {
         object[] GetParameterList(ConsumerMethod method, BasicDeliverEventArgs eventArgs, IModel model);
    }
}