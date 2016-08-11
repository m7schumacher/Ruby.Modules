using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Swiss;
using Ruby.Internal;

namespace Ruby.Immune
{
    public class ImmuneGate
    {
        private Generator generator;
        private Reader reader;

        internal static AntibodyNetwork Network { get; set; }

        public ImmuneGate()
        {
            Extensions.Initialize();

            reader = new Reader();
            generator = new Generator();
        }

        public void Initialize(bool generate = false)
        {
            Network = generate ? generator.GenerateNetwork() : generator.ReadNetwork();
        }

        public Antigen ProcessInput(string input)
        {
            var invader = new Antigen(input);
            Core.External.UserInput = invader.Text;
            Core.External.Search = invader.Search;

            return invader;
        }

        public string DetermineWhatToDo(Antigen invader)
        {
            string input = invader.Text;
            Command cmd = Core.Memory.Commands.FirstOrDefault(cm => cm.Text.EqualsIgnoreCase(input));

            if (cmd != null) return cmd.ID;
            else return string.Empty;
        }

        public void Learn(string id)
        {
            generator.Learn(id);
        }

        public void Output()
        {
            Network.OutputNetwork("Files/network.txt");
        }
    }
}
