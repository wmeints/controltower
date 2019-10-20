using Akka.Actor;

namespace ControlTower.Printer.Messages
{
    public class ConnectProtocol
    {
        public ConnectProtocol(IActorRef transport)
        {
            Transport = transport;
        }

        public IActorRef Transport { get; }
    }
}