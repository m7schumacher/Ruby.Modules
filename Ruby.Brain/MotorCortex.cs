using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruby.Movements;
using Ruby.Internal;

namespace Ruby.Mind
{
    public class MotorCortex : Lobe
    {
        internal static MuscleGate Muscle { get; set; }

        public MotorCortex() : base()
        {
            InitializeValues();
        }

        public override void InitializeValues()
        {
            Muscle = new MuscleGate();
        }

        public string React(string key)
        {
            string result = Muscle.React(key);
            return result;
        }

        public void GatherCommands()
        {
            var phrases = Muscle.GetRecognizers();

            foreach (var id in phrases.Keys)
            {
                var recognizers = phrases[id];

                foreach (string key in recognizers.Keys)
                {
                    foreach (string term in recognizers[key])
                    {
                        Core.Memory.Commands.Add(new Command() { Text = key.Replace("{}", term), ID = id });
                    }
                }
            }
        }

        public void PauseSpotify()
        {

        }

        public void ResumeSpotify()
        {

        }
    }
}
