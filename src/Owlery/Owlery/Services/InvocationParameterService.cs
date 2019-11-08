using System.Collections.Generic;
using Owlery.Models;
using Owlery.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Owlery.Services
{
    public class InvocationParameterService : IInvocationParameterService
    {
        private readonly IByteConversionService byteConversionService;

        public InvocationParameterService(IByteConversionService byteConversionService)
        {
            this.byteConversionService = byteConversionService;
        }

        public object[] GetParameterList(ConsumerMethod method, BasicDeliverEventArgs eventArgs, IModel model)
        {
            List<object> paramList = new List<object>();
            foreach (var param in method.Method.GetParameters())
            {
                if (param.IsDefined(typeof(FromBodyAttribute), false))
                {
                    paramList.Add(this.byteConversionService.ConvertFromByteArray(eventArgs.Body, param.ParameterType));
                }
                else if (param.IsDefined(typeof(FromDeliveryTagAttribute), false))
                {
                    paramList.Add(eventArgs.DeliveryTag);
                }
                else if (param.IsDefined(typeof(FromModelAttribute), false))
                {
                    paramList.Add(model);
                }
                else if (param.IsDefined(typeof(FromBasicPropertiesAttribute), false))
                {
                    paramList.Add(eventArgs.BasicProperties);
                }
                else if (param.IsDefined(typeof(FromConsumerTagAttribute), false))
                {
                    paramList.Add(eventArgs.ConsumerTag);
                }
                else if (param.IsDefined(typeof(FromExchangeAttribute), false))
                {
                    paramList.Add(eventArgs.Exchange);
                }
                else if (param.IsDefined(typeof(FromRedeliveredAttribute), false))
                {
                    paramList.Add(eventArgs.Redelivered);
                }
                else if (param.IsDefined(typeof(FromRoutingKeyAttribute), false))
                {
                    paramList.Add(eventArgs.RoutingKey);
                }
            }

            return paramList.ToArray();
        }
    }
}