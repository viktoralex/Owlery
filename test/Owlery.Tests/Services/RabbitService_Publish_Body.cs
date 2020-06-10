using Moq;
using Owlery.HostedServices;
using Owlery.Services;
using RabbitMQ.Client;
using Xunit;

namespace Owlery.Tests.Services
{
    public class RabbitService_Publish_Body
    {
        private readonly Mock<IRabbitModelAccessor> mockModelAccessor;
        private readonly Mock<IByteConversionService> mockConverter;
        private readonly Mock<IBasicPropertiesHandler> mockPropertiesHandler;

        private readonly Mock<IModel> mockModel;
        private readonly Mock<IBasicProperties> mockBasicProperties;

        public RabbitService_Publish_Body()
        {
            this.mockModelAccessor = new Mock<IRabbitModelAccessor>();
            this.mockConverter = new Mock<IByteConversionService>();
            this.mockBasicProperties = new Mock<IBasicProperties>();

            this.mockModel = new Mock<IModel>();
            this.mockPropertiesHandler = new Mock<IBasicPropertiesHandler>();

            this.mockModelAccessor.Setup(
                conn => conn.GetModel()
            ).Returns(this.mockModel.Object);

            this.mockModel.Setup(
                mod => mod.CreateBasicProperties()
            ).Returns(this.mockBasicProperties.Object);
        }

        [Fact]
        public void ShouldConvertBody()
        {
            // GIVEN - A rabbit service and a byte conversion service
            var body = new object();

            var service = new RabbitService(
                this.mockModelAccessor.Object,
                this.mockConverter.Object,
                this.mockPropertiesHandler.Object,
                TestLogger.Create<RabbitService>()
            );

            // WHEN - Publish is called with a body object
            service.Publish(body, "", "");

            // THEN - The byte conversion service should have been called with the body
            this.mockConverter.Verify(
                con => con.ConvertToByteArray(
                    It.Is<object>(ob => ob == body)
                )
            );
            this.mockConverter.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldCallPublish()
        {
            // GIVEN - A rabbit service and a byte conversion service
            var routingKey =  "routing-key";
            var exchange = "exchange";

            var service = new RabbitService(
                this.mockModelAccessor.Object,
                this.mockConverter.Object,
                this.mockPropertiesHandler.Object,
                TestLogger.Create<RabbitService>()
            );

            // WHEN - Publish is called with a body object
            service.Publish("body", routingKey, exchange);

            // THEN - The byte conversion service should have been called with the body
            this.mockModel.Verify(mod => mod.CreateBasicProperties());
            this.mockModel.Verify(mod => mod.Close());
            this.mockModel.Verify(
                mod => mod.BasicPublish(
                    It.Is<string>(s => s == exchange),
                    It.Is<string>(s => s == routingKey),
                    It.Is<bool>(b => b == false),
                    It.IsAny<IBasicProperties>(),
                    It.IsAny<byte[]>()
                )
            );
            this.mockModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldApplySettingsToProperties()
        {
            // GIVEN - A rabbit service and a byte conversion service
            var service = new RabbitService(
                this.mockModelAccessor.Object,
                this.mockConverter.Object,
                this.mockPropertiesHandler.Object,
                TestLogger.Create<RabbitService>()
            );

            // WHEN - Publish is called with a body object
            service.Publish("body", "routing-key", "exchange");

            // THEN - The byte conversion service should have been called with the body
            this.mockPropertiesHandler.Verify(
                propHand => propHand.ApplySettingsProperties(
                    It.IsAny<IBasicProperties>()
                )
            );
            this.mockPropertiesHandler.VerifyNoOtherCalls();
        }
    }
}