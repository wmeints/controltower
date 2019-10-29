using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.IO;

namespace ControlTower.Printer
{
    /// <summary>
    /// Contains status information about a possibly active print job
    /// </summary>
    public class PrintJobStatus
    {
        private int _totalSteps;
        private int _stepsCompleted;
        private PrintJobState _state;

        /// <summary>
        /// Gets the total number of steps in the current job
        /// </summary>
        public int TotalSteps
        {
            get => _totalSteps;
            set
            {
                _totalSteps = value;
                JobStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the completed number of steps in the job
        /// </summary>
        public int StepsCompleted
        {
            get => _stepsCompleted;
            set
            {
                _stepsCompleted = value;
                JobStatusChanged?.Invoke(this,EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the state of the print job
        /// </summary>
        public PrintJobState State
        {
            get => _state;
            set
            {
                _state = value;
                JobStatusChanged?.Invoke(this,EventArgs.Empty);
            }
        }

        public EventHandler JobStatusChanged;
    }
}
