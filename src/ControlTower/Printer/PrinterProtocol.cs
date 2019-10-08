using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implements the protocol layer for the printer.
    /// <para>
    /// The protocol layer is responsible for processing incoming commands from the printer
    /// to a format that is understood by the transport layer. It is also responsible for ordered processing
    /// of commands and replies.
    /// </para>
    /// </summary>
    public class PrinterProtocol : IPrinterProtocol
    {
        private static readonly Regex SplitPattern = new Regex("\\s+");
        private static readonly Regex ResendPattern = new Regex("N:?|:");

        private readonly ILogger<PrinterProtocol> _logger;
        private readonly IPrinterTransport _transport;
        private readonly Semaphore _sendSemaphore;
        private readonly Semaphore _receiveSemaphore;
        private readonly Semaphore _queueSemaphore;
        private readonly Queue<PrinterCommand> _commandQueue;
        private readonly Thread _sendingThread;
        private readonly Thread _receivingThread;
        private readonly Dictionary<int, PrinterCommand> _transmittedLines;
        
        private bool _active = true;
        private bool _waitingForReply;
        private bool _resending;
        private int _resendLineNumber;
        private int _lineNumber;
        
        /// <summary>
        /// Initializes a new instance of <see cref="PrinterProtocol"/>
        /// </summary>
        /// <param name="printerTransport">Transport layer to use</param>
        /// <param name="logger">Logger to use</param>
        public PrinterProtocol(IPrinterTransport printerTransport, ILogger<PrinterProtocol> logger)
        {
            _logger = logger;
            _transport = printerTransport;
            _sendSemaphore = new Semaphore(1, 1);
            _receiveSemaphore = new Semaphore(0, 1);
            _queueSemaphore = new Semaphore(0, 1);
            _commandQueue = new Queue<PrinterCommand>();
            _transmittedLines = new Dictionary<int, PrinterCommand>();
            _sendingThread = new Thread(ReceiveDataFromApplication);
            _receivingThread = new Thread(ReceiveDataFromTransport);
        }

        /// <summary>
        /// Gets raised when a command was successfully processed.
        /// </summary>
        public event EventHandler CommandProcessed;

        /// <summary>
        /// Connects the protocol to the transport layer and starts the read/write logic.
        /// </summary>
        public void Connect()
        {
            _transport.Connect();

            _active = true;

            _sendingThread.Start();
            _receivingThread.Start();
        }

        /// <summary>
        /// Disconnects from the transport layer and stops processing commands and replies.
        /// </summary>
        public void Disconnect()
        {
            _transport.Disconnect();

            _active = false;
        }

        /// <summary>
        /// Sends a command to the printer and waits for a reply
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="line">Line number (optional)</param>
        /// <param name="calculateChecksum">Whether a checksum should be calculated</param>
        public void SendCommand(string text, int line = -1, bool calculateChecksum = false)
        {
            _commandQueue.Enqueue(new PrinterCommand(text, calculateChecksum, line));

            _queueSemaphore.Release();
            _waitingForReply = true;
        }

        /// <summary>
        /// Calculates the checksum for a command.
        /// </summary>
        /// <param name="text">Command text to get the checksum for.</param>
        /// <returns>Returns the calculated checksum.</returns>
        private int CalculateChecksum(string text)
        {
            int checksum = text[0];

            if (text.Length > 1)
            {
                for (int i = 1; i < text.Length; i++)
                {
                    checksum = checksum ^ text[i];
                }
            }

            return checksum;
        }

        /// <summary>
        /// This reads responses from the transport layer.
        /// <para>
        /// Empty lines and debug responses are automatically skipped.
        /// We'll keep on reading data until we receive a line that we can process.
        /// </para>
        /// </summary>
        private void ReceiveDataFromTransport()
        {
            while (_active)
            {
                _receiveSemaphore.WaitOne();

                while (_waitingForReply)
                {
                    var line = _transport.ReadLine();

                    if (!string.IsNullOrEmpty(line.Trim()) && !line.StartsWith("DEBUG_"))
                    {
                        ProcessResponse(line);
                        _waitingForReply = false;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the response received from the transport layer.
        /// <para>
        /// We're raising specific events for responses that we receive from the transport layer.
        /// This allows the printer to respond accordingly.
        /// </para>
        /// <para>
        /// When we receive an <c>ok</c>, <c>start</c> or <c>Gbrl</c> response we'll send the
        /// <see cref="CommandProcessed"/> event to the printer.
        /// </para>
        /// <para>
        /// When we receive the <c>Error</c> response from the printer we'll extract the message
        /// from the response and kill the protocol layer. The printer normally doesn't send error replies
        /// so we have to stop regardless of the error message.
        /// </para>
        /// <para>
        /// When we receive a <c>resend</c> or <c>rsnd</c> response it means we messed up the print job
        /// and need to resubmit commands to the printer starting from the specified line number. We'll
        /// send a <see cref="ResendRequested"/> event for this to the printer.
        /// </para>
        /// </summary>
        /// <param name="line">The response received from the transport layer</param>
        /// <exception cref="ProtocolException">Gets thrown when a protocol failure is detected.</exception>
        private void ProcessResponse(string line)
        {
            // This section deals with acknowledgements from the printer.
            // When we receive one of these messages, we're cleared to send data.
            if (line.StartsWith("start") || line.StartsWith("Gbrl ") || line.StartsWith("ok"))
            {
                //TODO: Confirm command to printer
                _sendSemaphore.Release();
            }

            // This section deals with the temperature callbacks from the printer.
            if (line.StartsWith("ok") && line.Contains("T:"))
            {
                //TODO: Process temperature read-outs
                _sendSemaphore.Release();
            }

            // This section deals with errors received from the printer.
            // Usually, when this happens we need to stop the printer communication.
            if (line.StartsWith("Error"))
            {
                _logger.LogError("Received error from printer: {ErrorMessage}", line);
                throw new ProtocolException(line);
            }

            // When we receive a resend callback, we'll switch the protocol to resending previously sent
            // commands. Other commands will have to wait in line until we've transmitted all remaining commands.
            if (line.ToLower().StartsWith("resend") || line.StartsWith("rs"))
            {
                line = ResendPattern.Replace(line, " ");
                var words = SplitPattern.Split(line).ToList();

                while (words.Count > 0)
                {
                    var word = words[0];
                    words.RemoveAt(0);

                    if (int.TryParse(word, out var resendIndex))
                    {
                        RequestResend(resendIndex);
                        break;
                    }
                }

                _sendSemaphore.Release();
            }
        }

        /// <summary>
        /// Switches the protocol to resend mode and fires the <see cref="ResendRequested"/> event.
        /// <para>
        /// The protocol switches back to normal operation mode once the line counter has gone beyond
        /// the value that it had before processing the resend request.
        /// </summary>
        /// <param name="resendIndex">Line number to start resending at</param>
        private void RequestResend(int resendIndex)
        {
            _resending = true;
            _resendLineNumber = resendIndex;
        }

        /// <summary>
        /// Processes the command queue and sents the commands to the transport layer.
        /// <para>
        /// The printer protocol is only allowed to send a command when it has received a reply
        /// for the previous command. We're using a semaphore to coordinate this.
        /// </para>
        /// <para>
        /// Commands are pulled from the command queue until it is empty. We'll wait for new commands
        /// using another semaphore that is signalled when a new command is submitted to the protocol layer
        /// through the <see cref="SendCommand"/> method.
        /// </para>
        /// </summary>
        private void ReceiveDataFromApplication()
        {
            while (_active)
            {
                if (_resending)
                {
                    ResendCommands();
                }
                else
                {
                    WaitForAvailableCommands();
                    ProcessCommandQueue();
                }
            }
        }

        /// <summary>
        /// Resends commands from a specified point in time until the most recent command
        /// that was sent with a line number.
        /// </summary>
        private void ResendCommands()
        {
            while (_resendLineNumber < _lineNumber)
            {
                ProcessCommand(_transmittedLines[_resendLineNumber]);
                _resendLineNumber++;
            }

            _resending = false;
        }

        /// <summary>
        /// Processes all commands that are waiting in the queue and transmits them to the transport layer.
        /// </summary>
        private void ProcessCommandQueue()
        {
            while (_commandQueue.Count > 0 && !_resending)
            {
                _sendSemaphore.WaitOne();

                var cmd = _commandQueue.Dequeue();

                ProcessCommand(cmd);
            }
        }

        /// <summary>
        /// Waits for new commands to arrive in the command queue.
        /// Automatically continues when there are commands in the queue.
        /// </summary>
        private void WaitForAvailableCommands()
        {
            if (_commandQueue.Count == 0)
            {
                _queueSemaphore.WaitOne();
            }
        }

        /// <summary>
        /// Processes a single command.
        /// <para>
        /// When printing from a job we're using line numbers and checksumming to make sure that the print
        /// job is executed the way we specified it in the g-code file. For this purpose we send a specific
        /// format of N[linenumber] [text]*[checksum].
        /// </para>
        /// <para>
        /// Besides printing from a job we also allow sending singular commands to the printer.
        /// In this case we're not generating checksums and counting line numbers.
        /// </para>
        /// <para>
        /// Please note that we're only using checksumming when the transport layer requests it.
        /// </para>
        /// </summary>
        /// <param name="cmd">Command to process</param>
        private void ProcessCommand(PrinterCommand cmd)
        {
            RecordCommandForResend(cmd);

            var line = cmd.Text;

            if (cmd.CalculateChecksum && _transport.RequiresChecksum)
            {
                var prefix = $"N{cmd.LineNumber} {cmd.Text}";
                var checksum = CalculateChecksum(prefix);

                line = $"{prefix}*{checksum}";
            }

            _transport.WriteLine(line);

            _receiveSemaphore.Release();
        }

        /// <summary>
        /// Records commands with a line number so we can resend them when needed.
        /// </summary>
        /// <param name="cmd">Command to record.</param>
        private void RecordCommandForResend(PrinterCommand cmd)
        {
            if (cmd.LineNumber > 0 && !_resending)
            {
                _transmittedLines[cmd.LineNumber] = cmd;
                _lineNumber = cmd.LineNumber;
            }
        }
    }
}