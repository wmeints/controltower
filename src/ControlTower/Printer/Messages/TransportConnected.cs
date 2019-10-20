using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    public class TransportConnected
    {
        public static TransportConnected Instance { get; } = new TransportConnected();
    }
}
