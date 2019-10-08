using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ControlTower.Printer
{
    /// <summary>
    /// Provides a transport to a virtual printer for integration testing
    /// </summary>
    public sealed class VirtualPrinterPrinterTransport : IPrinterTransport, IDisposable
    {
        private readonly TcpClient _client;
        private readonly string _address;
        private readonly int _port;

        private StreamReader _reader;
        private StreamWriter _writer;
        
        /// <summary>
        /// Initializes a new instance of <see cref="VirtualPrinterPrinterTransport"/>
        /// </summary>
        /// <param name="address">Address to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        public VirtualPrinterPrinterTransport(string address, int port)
        {
            _address = address;
            _port = port;
            _client = new TcpClient();
        }
        
        /// <summary>
        /// Opens the connection to the printer
        /// </summary>
        public void Connect()
        {
            _client.Connect(_address,_port);

            var stream = _client.GetStream();
            
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);
        }

        /// <summary>
        /// Disconnects from the printer
        /// </summary>
        public void Disconnect()
        {
            _reader.Close();
            _client.Close();
        }

        /// <summary>
        /// Reads data from the transport until a linefeed or carriage return character is found.
        /// </summary>
        /// <returns>Returns the retrieved data</returns>
        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        /// <summary>
        /// Disposes unmanaged resources
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
            _reader.Dispose();
        }
        
        /// <summary>
        /// Writes a single line of data to the transport
        /// </summary>
        /// <param name="text">Text to send</param>
        public void WriteLine(string text)
        {
            _writer.WriteLine(text);
        }
        
        /// <summary>
        /// Gets whether the printer protocol needs to generate checksums for the transport
        /// </summary>
        public bool RequiresChecksum => false;
    }
}