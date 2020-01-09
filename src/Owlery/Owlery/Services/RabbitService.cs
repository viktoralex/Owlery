using Owlery.HostedServices;
using Owlery.Utils;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public class RabbitService : IRabbitService
    {
        private readonly RabbitConnection rabbitConnection;
        private readonly IByteConversionService byteConversionService;

        public RabbitService(
            RabbitConnection rabbitConnection,
            IByteConversionService byteConversionService)
        {
            this.rabbitConnection = rabbitConnection;
            this.byteConversionService = byteConversionService;
        }

        public void Publish(string routingKey, object body, string exchange = null, IBasicProperties basicProperties = null)
        {
            var bodyBytes = this.byteConversionService.ConvertToByteArray(body);

            if (exchange == null)
                exchange = "";

            using (var model = rabbitConnection.GetModel())
            {
                model.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: basicProperties,
                    body: bodyBytes
                );
            }
        }
    }
}