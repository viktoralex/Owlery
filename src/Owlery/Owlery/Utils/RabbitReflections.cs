using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Owlery.Models;

namespace Owlery.Utils
{
    public static class RabbitReflections
    {
        public static IEnumerable<RabbitConsumerMethod> GetControllerConsumerMethods()
        {
            var methods = new List<RabbitConsumerMethod>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsDefined(typeof(RabbitControllerAttribute), false))
                    {
                        foreach (var method in type.GetMethods().Where(
                            m => m.IsDefined(typeof(RabbitConsumerAttribute), false)))
                        {
                            var consumerAttribute = (RabbitConsumerAttribute)method.GetCustomAttributes().Where(
                                attr => attr.GetType() == typeof(RabbitConsumerAttribute)
                            ).First();
                            var publisherAttribute = (RabbitPublisherAttribute)method.GetCustomAttributes().Where(
                                attr => attr.GetType() == typeof(RabbitPublisherAttribute)
                            ).FirstOrDefault();

                            methods.Add(
                                new RabbitConsumerMethod(
                                    method, type, consumerAttribute, publisherAttribute));
                        }
                    }
                }
            }

            return methods;
        }
    }
}