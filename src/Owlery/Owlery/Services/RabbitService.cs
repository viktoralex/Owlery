using Owlery.HostedServices;
using Owlery.Utils;

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

        public void Publish(string routingKey, object body, string exchange = null)
        {
            var bodyBytes = this.byteConversionService.ConvertToByteArray(body);

            var model = rabbitConnection.GetModel();
            model.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: null,
                body: bodyBytes
            );
        }
    }
}