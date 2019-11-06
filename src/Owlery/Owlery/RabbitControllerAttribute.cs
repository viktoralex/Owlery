using System;

namespace Owlery
{
    public class RabbitControllerAttribute : Attribute
    {
        string vhost;

        public RabbitControllerAttribute()
        {
            this.vhost = "";
        }

        public RabbitControllerAttribute(string vhost)
        {
            this.vhost = vhost;
        }
    }
}