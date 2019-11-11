Owlery
======

![GitHub](https://img.shields.io/github/license/viktoralex/owlery)
[![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/viktoralex/owlery?label=github&color=informational)](https://github.com/viktoralex/Owlery)
[![Nuget](https://img.shields.io/nuget/v/owlery?color=informational)](https://www.nuget.org/packages/Owlery)

The Owlery library provides .NET Core developers the ability to consume and
publish to RabbitMQ queues with the same ease as creating a new ApiController.

# Examples

The following are a few examples of how Owlery can help you.

## Defining queue consumers

The following is added in the `ConfigureServices`  method in the `Startup.cs` 
file.

```C#
using Owlery.Extensions;

...

public void ConfigureServices(IServiceCollection services)
{
    services.AddRabbitControllers();
    ...
}
```

Then we create two consumers in a controller class. The `textConsumer` method
consumes messages from the `tester.test` queue and creates a new instance of the
`AModel` class. This is then published to the `tester.another.test` queue via
the `testExc` exchange. The `anotherTestConsumer` method consumes messages for
`tester.another.test` and logs the `AModel` instances it recieves.

```C#
using Microsoft.Extensions.Logging;
using Owlery;

[RabbitController]
public class OwleryController
{
    private readonly ILogger logger;

    public OwleryController(ILogger<OwleryController> logger)
    {
        this.logger = logger;
    }

    [RabbitConsumer(queueName: "tester.test")]
    [RabbitPublisher(routingKey: "tester.another.test")]
    public AModel testConsumer()
    {
        logger.LogInformation($"Creating model");
        return new AModel {
            AnInteger = 99,
            AString = "This is a string",
        };
    }

    [RabbitConsumer(queueName: "tester.another.test")]
    public void anotherTestConsumer([FromBody] AModel aModel)
    {
        logger.LogInformation($"Received a model {aModel}");
    }
}

public class AModel
{
    public int AnInteger { get; set; }
    public string AString { get; set; }

    public override string ToString()
    {
        return $"[AModel AnInteger: {this.AnInteger}, AString: {this.AString}]";
    }
}
```

## Publishing to an exchange

To publish a message to an exchange Dependency Injection is used. First we
require the IRabbitService in the constructor.

```C#
using Owlery.Services;

...

private readonly IRabbitService rabbitService;

public ClassConstructor(
    IRabbitService rabbitService)
{
    this.rabbitService = rabbitService;
}
```

Then we can use the `Publish` method to publish the message

```C#
this.rabbitService.Publish(routingKey: "tester.test", body: "Message body", exchange: "");
```

But we can use any JSONSerializable object as the body, or an array of bytes directly.

## Declaring queues

Automatically declare queues, exchanges and bind queues to exchanges when the 
application starts using configuration only.

In `appsettings.json` or any other configuration source define the following

```json
{
  ...
  "Owlery": {
    "Queues": [
      {
        "QueueName": "tester.test"
      },
      {
        "QueueName": "tester.another.test"
      }
    ],
    "Exchanges": [
      {
        "ExchangeName": "testExc"
      }
    ],
    "Bindings": [
      {
        "QueueName": "tester.another.test",
        "ExchangeName": "testExc",
        "RoutingKey": "tester.routingKey"
      }
    ]
  }
}
```

The following is added in the `ConfigureServices`  method in the `Startup.cs` 
file.

```C#
using Owlery.Extensions;

...

public void ConfigureServices(IServiceCollection services)
{
    services.AddRabbitControllers(Configuration.GetSection("Owlery"));
    ...
}
```