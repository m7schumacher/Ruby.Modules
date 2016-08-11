using Swiss.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ruby.Immune;
using Swiss;
using Ruby.Internal;

namespace Ruby.Mind
{
    public class SensoryCortex : Lobe
    {
        internal Antigen Invader { get; set; }
        internal Situation Situation { get; set; }
        internal ImmuneGate Immune { get; set; }

        public SensoryCortex() : base()
        {
            InitializeValues();
        }

        public override void InitializeValues()
        {
            Immune = new ImmuneGate();
        }

        public string Process(string input)
        {
            Invader = Immune.ProcessInput(input);

            string key = string.Empty;

            if (Invader.IsTargeting)
            {
                key = Invader.Target;
                Immune.Learn(key);
            }
            else
            {
                key = Core.Memory.CheckIfSeenBefore(input);

                if(string.IsNullOrEmpty(key))
                {
                    key = Immune.DetermineWhatToDo(Invader);
                }
            }

            return key;
        }
    }
}
