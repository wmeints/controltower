using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    public class DisconnectDevice
    {
        public static DisconnectDevice Instance { get; } = new DisconnectDevice();
    }
}
