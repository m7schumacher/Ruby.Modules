using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ruby.Internal;

namespace Ruby.Muscle
{
    internal class Terminator : Reflex
    {
        public Terminator() : base() 
        {
            Name = "Terminate";
            ID = "0";
        }

        public override string Execute()
        {
            RefreshSettings();
            Environment.Exit(0);

            return "Goodbye sir";
        }

        private void RefreshSettings()
        {
            string[] freshLines = new string[]
            {
                "quiet=" + Core.Internal.isQuiet,
                "dreaming=" + Core.Internal.isDreaming
            };

            File.WriteAllLines(Core.FilePaths.SettingsFile, freshLines);
        }
    }
}
