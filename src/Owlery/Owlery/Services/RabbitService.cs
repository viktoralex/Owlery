using Owlery.HostedServices;
using Owlery.Utils;

namespace Owlery.Services
{
    public class RabbitService : IRabbitService
    {
        private readonly RabbitConnection rabbitConnection;

        public RabbitService(
            RabbitConnection rabbitConnection)
        {
            this.rabbitConnection = rabbitConnection;
        }

        public void Publish(string routingKey, object body, string exchange = null)
        {
            var bodyBytes = BodyConverter.ConvertToByteArray(body);

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