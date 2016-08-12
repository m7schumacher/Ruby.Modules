using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;

namespace Ruby.Internal
{
    public class MachineCortex
    {
        public List<Process> RunningProcesses { get; set; }

        public double MemoryAvailable { get; set; }
        public double CPUAvailable { get; set; }
        public int NumberOfProcesses { get { return RunningProcesses.Count; } }

        private Timer _updater;

        public MachineCortex()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            Task tsk = Task.Factory.StartNew(() =>
            {
                _updater = new Timer(30000);
                _updater.Elapsed += new ElapsedEventHandler(UpdateMetrics);
                _updater.Enabled = true;
            }); 
        }

        public void UpdateMetrics(object sender, ElapsedEventArgs e)
        {
            RunningProcesses = Process.GetProcesses().ToList();
        }
    }
}
