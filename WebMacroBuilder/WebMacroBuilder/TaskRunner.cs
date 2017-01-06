using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMacroBuilder
{
    public enum RunnerState
    {
        Started,
        Paused,
        Stopped
    }

    public class TaskRunner
    {
        public RunnerState RunningState { get; set; }

        public List<ICommandButton> Commands { get; set; }

        public TaskRunner()
        {

        }

        public TaskRunner(List<ICommandButton> commands)
        {

        }
    }
}
