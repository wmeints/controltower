using System;
using System.Collections.Generic;
using System.Text;
using Akka.TestKit.Xunit2;
using ControlTower.Printer;
using ControlTower.Printer.Messages;
using Xunit;

namespace ControlTower.Tests
{
    public class PrintJobTests : TestKit
    {
        [Fact]
        public void DoesScheduleNextCommandAfterReceivingNotification()
        {
            var printer = CreateTestProbe();
            var printerMonitor = CreateTestProbe();

            var commands = new List<PrinterCommand>()
            {
                new PrinterCommand(1, "M104 S40"),
                new PrinterCommand(2, "M190")
            };

            var job = ActorOf(PrintJob.Props(commands, printer.Ref, printerMonitor));

            job.Tell(StartPrinting.Instance, TestActor);
            printer.ExpectMsg<PrinterCommand>(cmd => cmd.Text == "M104 S40");

            job.Tell(PrinterCommandProcessed.Instance, printer.Ref);
            printer.ExpectMsg<PrinterCommand>(cmd => cmd.Text == "M190");
        }

        [Fact]
        public void CanReportStatusInformation()
        {
            var printer = CreateTestProbe();
            var printerMonitor = CreateTestProbe();

            var commands = new List<PrinterCommand>()
            {
                new PrinterCommand(1, "M104 S40"),
                new PrinterCommand(2, "M190")
            };

            var job = ActorOf(PrintJob.Props(commands, printer.Ref, printerMonitor.Ref));
            printerMonitor.ExpectMsg<PrintJobStatusUpdated>(cmd => cmd.State == PrintJobState.Ready);

            job.Tell(StartPrinting.Instance, TestActor);
            printerMonitor.ExpectMsg<PrintJobStatusUpdated>(cmd => cmd.State == PrintJobState.Running);
            printerMonitor.ExpectMsg<PrintJobStepsCompleted>(cmd => cmd.TotalSteps == 4 && cmd.StepsCompleted == 0);

            job.Tell(PrinterCommandProcessed.Instance, printer.Ref);
            printerMonitor.ExpectMsg<PrintJobStepsCompleted>(cmd => cmd.TotalSteps == 4 && cmd.StepsCompleted == 1);

            job.Tell(PrinterCommandProcessed.Instance, printer.Ref);
            printerMonitor.ExpectMsg<PrintJobStepsCompleted>(cmd => cmd.TotalSteps == 4 && cmd.StepsCompleted == 2);

            job.Tell(PrinterCommandProcessed.Instance, printer.Ref);
            printerMonitor.ExpectMsg<PrintJobStepsCompleted>(cmd => cmd.TotalSteps == 4 && cmd.StepsCompleted == 3);

            job.Tell(PrinterCommandProcessed.Instance, printer.Ref);
            printerMonitor.ExpectMsg<PrintJobStepsCompleted>(cmd => cmd.TotalSteps == 4 && cmd.StepsCompleted == 4);
            printerMonitor.ExpectMsg<PrintJobStatusUpdated>(cmd => cmd.State == PrintJobState.Completed);
        }
    }
}
