using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is used to report the number of completed steps in the job
    /// </summary>
    public class PrintJobStepsCompleted
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrintJobStepsCompleted"/>
        /// </summary>
        /// <param name="stepsCompleted">Number of steps completed</param>
        /// <param name="totalSteps">Total number of steps in the job</param>
        public PrintJobStepsCompleted(int stepsCompleted, int totalSteps)
        {
            StepsCompleted = stepsCompleted;
            TotalSteps = totalSteps;
        }

        /// <summary>
        /// Gets the number of completed steps
        /// </summary>
        public int StepsCompleted { get; }

        /// <summary>
        /// Gets the total number of steps in the job
        /// </summary>
        public int TotalSteps { get; }
    }
}
