using System.Threading;
using Castle.DynamicProxy.Generators.Emitters;
using ControlTower.Printer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Moq;
using Xunit;

namespace ControlTower.Tests
{
    public class PrinterProtocolTests
    {
        [Fact]
        public void PrinterProtocol_WhenTransmitting_ReceivesResponses()
        {
            var loggerFactory = new LoggerFactory(new [] { new DebugLoggerProvider() });
            var logger = loggerFactory.CreateLogger<PrinterProtocol>();
            
            Mock<IPrinterTransport> transport = new Mock<IPrinterTransport>();
            PrinterProtocol protocol = new PrinterProtocol(transport.Object, logger);
            
            transport.Setup(mock => mock.ReadLine()).Returns("ok");
            
            protocol.Connect();
            protocol.SendCommand("G0");
            
            transport.Verify(x => x.WriteLine(It.IsAny<string>()));
        }

        [Fact]
        public void PrinterProtocol_WhenReceivingResend_ResendsMissingCommands()
        {
            var loggerFactory = new LoggerFactory(new [] { new DebugLoggerProvider() });
            var logger = loggerFactory.CreateLogger<PrinterProtocol>();
            var triggerResend = false;
            
            Mock<IPrinterTransport> transport = new Mock<IPrinterTransport>();
            PrinterProtocol protocol = new PrinterProtocol(transport.Object, logger);

            transport.SetupGet(mock => mock.RequiresChecksum).Returns(true);
            
            transport.Setup(mock => mock.WriteLine(It.IsAny<string>())).Callback((string text) =>
            {
                if (text == "N2 G1*42")
                {
                    triggerResend = true;
                }
            });
            
            transport.Setup(mock => mock.ReadLine()).Returns(() =>
            {
                if (triggerResend)
                {
                    triggerResend = false;
                    return "rsnd N1";
                }

                return "ok";
            });
            
            transport.Setup(mock => mock.ReadLine()).Returns("ok");
            
            protocol.Connect();
            protocol.SendCommand("G0",1, true);
            protocol.SendCommand("G1", 2, true);
            
            Thread.Sleep(100);
            
            transport.Verify(x => x.WriteLine("N1 G0*40"));
            transport.Verify(x => x.WriteLine("N2 G1*42"));
            transport.Verify(x => x.WriteLine("N1 G0*40"));
            transport.Verify(x => x.WriteLine("N2 G1*42"));
        }
    }
}