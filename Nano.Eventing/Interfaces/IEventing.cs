using System;
using System.Threading.Tasks;

namespace Nano.Eventing.Interfaces
{
    /// <summary>
    /// Eventing interface.
    /// </summary>
    public interface IEventing : IDisposable
    {
        /// <summary>
        /// Publishes a message to an exchange using 'fanout' publish/subscribe topology.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="routing">The routing key (if any).</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task PublishAsync<TMessage>(TMessage body, string routing = "")
            where TMessage : class;

        /// <summary>
        /// Consumes messages.
        /// </summary>
        /// <typeparam name="TMessage">The type of response body.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <param name="routing">The routing key (if any).</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task SubscribeAsync<TMessage>(IServiceProvider serviceProvider, string routing = "")
            where TMessage : class;
    }
}