namespace Owlery.Models
{
    /// <summary>
    /// Different ways of acknowledging receipt of message from RabbitMQ. 
    /// </summary>
    public enum AcknowledgementType
    {
        /// <summary>
        /// Acknowledge immediately.
        /// </summary>
        AutoAck,
        /// <summary>
        /// Acknowledge on successful invocation of the consumer function.
        /// </summary>
        AckOnInvoke,
        /// <summary>
        /// Acknowledge on successful publish of response.
        /// </summary>
        AckOnPublish,
        /// <summary>
        /// Don't acknowledge automatically, allow the consumer method to acknowledge.
        /// </summary>
        ManualAck,
    }
}