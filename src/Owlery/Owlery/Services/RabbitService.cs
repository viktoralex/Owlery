using Owlery.HostedServices;
using Owlery.Models;
using Owlery.Utils;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public class RabbitService : IRabbitService
    {
        private readonly IRabbitModelAccessor rabbitModelAccessor;
        private readonly IByteConversionService byteConversionService;
        private readonly IBasicPropertiesHandler basicPropertiesHandler;

        public RabbitService(
            IRabbitModelAccessor rabbitModelAccessor,
            IByteConversionService byteConversionService,
            IBasicPropertiesHandler basicPropertiesHandler)
        {
            this.rabbitModelAccessor = rabbitModelAccessor;
            this.byteConversionService = byteConversionService;
            this.basicPropertiesHandler = basicPropertiesHandler;
        }

        public void Publish(object body, string routingKey, string exchange = null)
        {
            var bodyBytes = this.byteConversionService.ConvertToByteArray(body);

            if (exchange == null)
                exchange = "";

            var model = rabbitModelAccessor.GetModel();

            var basicProperties = model.CreateBasicProperties();
            this.basicPropertiesHandler.ApplySettingsProperties(basicProperties);

            model.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: basicProperties,
                body: bodyBytes
            );

            model.Close();
        }

        public void Publish(RabbitMessage message, string routingKey, string exchange = null)
        {
            var bodyBytes = this.byteConversionService.ConvertToByteArray(message.Body);

            if (exchange == null)
                exchange = "";

            var model = rabbitModelAccessor.GetModel();

            var basicProperties = model.CreateBasicProperties();
            this.basicPropertiesHandler.ApplySettingsProperties(basicProperties);
            this.basicPropertiesHandler.ApplyMessageProperties(message, basicProperties);

            model.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: basicProperties,
                body: bodyBytes
            );

            model.Close();
        }
    }
}