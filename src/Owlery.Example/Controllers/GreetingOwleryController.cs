using Microsoft.Extensions.Options;
using Owlery.Models;
using Owlery.Models.Settings;
using Owlery.Services;

namespace Owlery.Example.Controllers
{
    [RabbitController]
    public class GreetingOwleryController
    {
        private readonly IRabbitService rabbitService;

        public GreetingOwleryController(
            IRabbitService rabbitService)
        {
            this.rabbitService = rabbitService;
        }

        [RabbitConsumer(queueName: "{Owlery:Queues:HelloConsumer:QueueName}",
            acknowledgementType: AcknowledgementType.AckOnPublish)]
        [RabbitPublisher(
            exchangeName: "{Owlery:Bindings:GreetingResult:ExchangeName}",
            routingKey: "{Owlery:Bindings:GreetingResult:RoutingKey}")]
        public GreetingResult SayHello([FromBody] GreetingRequest greetingRequest)
        {
            return new GreetingResult() {
                Greeting = $"Hello, {greetingRequest.Name}!"
            };
        }

        [RabbitConsumer(queueName: "{Owlery:Queues:HiConsumer:QueueName}",
            acknowledgementType: AcknowledgementType.AutoAck)]
        public void SayHi([FromBody] GreetingRequest greetingRequest)
        {
            var body = new GreetingResult() {
                Greeting = $"Hi, {greetingRequest.Name}!"
            };

            var message = new RabbitMessage();
            message.Body = body;

            this.rabbitService.Publish(
                message: message,
                routingKey: "owlery.example.answer",
                exchange: "owlery.example.greeting_exc"
            );
        }
    }

    public class GreetingRequest
    {
        public string Name { get; set; }
    }

    public class GreetingResult
    {
        public string Greeting { get; set; }
    }
}