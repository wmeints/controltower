using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    public class ProtocolDisconnected
    {
        public static ProtocolDisconnected Instance { get; } = new ProtocolDisconnected();
    }
}
