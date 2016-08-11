using Ruby.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Muscle{
    internal class Googler : Reflex
    {
        public Googler() : base()
        {
            Name = "Googler";
        }

        public override string Execute()
        {
            string partial = Core.FilePaths.GoogleURL;

            Process firstProc = new Process();

            if (Core.External.Search.ToLower().Replace("google", "").Length > 0)
            {
                firstProc.StartInfo.FileName = partial + Core.External.Search.Replace("'", "");
            }
            else { firstProc.StartInfo.FileName = "https://www.google.com"; }

            firstProc.Start();

            return "performing a google search sir";
        }
    }
}
