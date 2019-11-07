namespace Owlery.Services
{
    public interface IRabbitService
    {
         void Publish(string routingKey, object body, string exchange = null);
    }
}