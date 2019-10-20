using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    public class TransportDisconnected
    {
        public static TransportDisconnected Instance { get; } = new TransportDisconnected();
    }
}
