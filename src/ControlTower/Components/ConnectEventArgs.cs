using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Components
{
    public class ConnectEventArgs: EventArgs
    {
        public ConnectEventArgs(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
        }

        public string PortName { get; }
        public int BaudRate { get; }
    }
}
