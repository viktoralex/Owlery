using RabbitMQ.Client;

namespace Owlery.Services
{
    public interface IDeclarationService
    {
         void DeclareAll(IModel model);
    }
}