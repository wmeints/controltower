using Akka.TestKit.Xunit2;
using ControlTower.Controller;
using ControlTower.Printer.Messages;
using Xunit;

namespace ControlTower.Tests
{
    public class PrinterProtocolTests: TestKit
    {
        [Fact]
        public void CanConnect()
        {
            var printer = CreateTestProbe();
            var protocol = ActorOf(PrinterProtocol.Props(printer));
            var transport = CreateTestProbe();
            
            protocol.Tell(new ConnectProtocol(transport), TestActor);
            transport.ExpectMsg<ConnectTransport>();
            
            protocol.Tell(DisconnectProtocol.Instance, TestActor);
            transport.ExpectMsg<DisconnectTransport>();
            
        }

        [Fact]
        public void CanSendAndReceiveCommands()
        {
            var protocol = ActorOf(PrinterProtocol.Props());
            var transport = CreateTestProbe();
            
            protocol.Tell(new ConnectProtocol(transport), TestActor);

            transport.ExpectMsg<ConnectTransport>();

            protocol.Tell(new PrinterCommand(1, "G0"), TestActor);
            protocol.Tell(new PrinterCommand(2, "G1"), TestActor);
            transport.ExpectMsg<PrinterCommand>(x => x.LineNumber == 1);
            
            protocol.Tell(new PrinterResponse("ok"), transport);
            
            protocol.Tell(new PrinterCommand(2, "G1"), TestActor);
            transport.ExpectMsg<PrinterCommand>(x=> x.LineNumber == 2);
        }

        [Fact]
        public void CanHandleResends()
        {
            var protocol = ActorOf(PrinterProtocol.Props());
            var transport = CreateTestProbe();
            
            protocol.Tell(new ConnectProtocol(transport), TestActor);
            transport.ExpectMsg<ConnectTransport>();
            
            protocol.Tell(new PrinterCommand(1, "G0"), TestActor);
            transport.ExpectMsg<PrinterCommand>(x => x.LineNumber == 1);
            
            protocol.Tell(new PrinterResponse("ok"), transport);
            
            protocol.Tell(new PrinterCommand(2, "G1"), TestActor);
            transport.ExpectMsg<PrinterCommand>(x => x.LineNumber == 2);
            
            protocol.Tell(new PrinterResponse("rsnd N:1"), transport);
            transport.ExpectMsg<PrinterCommand>(x => x.LineNumber == 1);
            
            protocol.Tell(new PrinterResponse("ok"), transport);
            transport.ExpectMsg<PrinterCommand>(x => x.LineNumber == 2);
            
            protocol.Tell(new PrinterResponse("ok"), transport);
        }

        [Fact]
        public void CanHandleAsyncTemperatureReports()
        {
            var protocol = ActorOf(PrinterProtocol.Props());
            var transport = CreateTestProbe();
            
            //TODO: Send a command and reply with an out-of-band temperature report + ok.
        }
    }
}