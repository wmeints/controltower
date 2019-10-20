using Akka.TestKit;
using Akka.TestKit.Xunit2;
using ControlTower.Controller;
using ControlTower.Printer.Messages;
using Xunit;

namespace ControlTower.Tests
{
    public class FlightRecorderTests: TestKit
    {
        [Fact]
        public void CanReplaySingleCommand()
        {
            var protocol = CreateTestProbe();
            var flightRecorder = ActorOf(FlightRecorder.Props(protocol));
            
            flightRecorder.Tell(new PrinterCommand(1, "G0"), protocol.Ref);
            flightRecorder.Tell(new ReplayCommands(1), protocol.Ref);
            
            protocol.AwaitAssert(() => protocol.ExpectMsg<ResendCommand>());
        }

        [Fact]
        public void CanReplayMultipleCommands()
        {
            var protocol = CreateTestProbe();
            var flightRecorder = ActorOf(FlightRecorder.Props(protocol));
            
            flightRecorder.Tell(new PrinterCommand(1, "G0"), protocol.Ref);
            flightRecorder.Tell(new PrinterCommand(2, "G0"), protocol.Ref);
            
            flightRecorder.Tell(new ReplayCommands(1), protocol.Ref);
            
            protocol.AwaitAssert(() => protocol.ExpectMsg<ResendCommand>(x => x.Command.LineNumber == 1));
            protocol.AwaitAssert(() => protocol.ExpectMsg<ResendCommand>(x => x.Command.LineNumber == 2));
        }
    }
}