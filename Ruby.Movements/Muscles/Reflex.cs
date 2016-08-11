using Swiss;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Muscle
{
    internal class Reflex : Response
    {
        public string Target { get; set; }

        public Reflex()
        {
            Target = string.Empty;
        }

        public Reflex(string target) : this()
        {
            Target = target;
        }

        public override string Execute()
        {
            if (!Swiss.ProcessUtility.IsProcessRunning(Target))
            {
                Process firstProc = new Process();
                firstProc.StartInfo.FileName = Target;
                firstProc.Start();
            }

            return string.Empty;
        }

        public override void EstablishRecognizers() { }
    }
}
