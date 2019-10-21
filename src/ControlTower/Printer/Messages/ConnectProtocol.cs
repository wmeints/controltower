using Akka.Actor;

namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Message used to connect the protocol layer
    /// </summary>
    public class ConnectProtocol
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="ConnectProtocol" />
        /// </summary>
        /// <param name="transport">Transport layer to connect to</param>
        public ConnectProtocol(IActorRef transport)
        {
            Transport = transport;
        }

        /// <summary>
        ///     Transport layer to connect to
        /// </summary>
        public IActorRef Transport { get; }
    }
}