using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Swiss;
using System.Diagnostics;
using Ruby.Internal;

namespace Ruby.Immune
{
    internal class Generator
    {
        private AntibodyNetwork Net { get; set; }

        public void Learn(string responderID)
        {
            //AddCellToNetwork(responderID);
            //AddTextToMemory(responderID);

            Net.WriteNetwork("Files/mind.bin");
            Net.OutputNetwork("Files/network.txt");
        }

        #region Antibodies

        public AntibodyNetwork GenerateNetwork()
        {
            Net = new AntibodyNetwork();

            HashSet<string> seen = new HashSet<string>();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Core.Memory.Commands.ExecuteInParallel((cm) =>
            {
                string sentence = Regex.Replace(cm.Text, "\"([^\"]*)\"", "par");
                sentence = Extensions.FilterUselessStrings(sentence);
                string cell = cm.ID;

                string[] bits = Extensions.TagAndBreak(sentence);
                bits = Extensions.FilterOnPOS(bits);

                foreach (string bit in bits)
                {
                    Antibody node = new Antibody(bit, cell);
                    Antibody twinChild = Net.GetMatchingNode(node);

                    if (twinChild != null)//if child has been seen before
                    {
                        twinChild.Merge(node);//merge with existing
                    }
                    else//if child has not been seen before
                    {
                        Net.AddNode(node);
                    }
                }
            }, 8);

            watch.Stop();

            var elapse = watch.Elapsed.TotalSeconds;

            Net.WriteNetwork("Files/mind.bin");

            return Net;
        }

        public AntibodyNetwork ReadNetwork()
        {
            ConcurrentBag<Antibody> net = (ConcurrentBag<Antibody>)FileUtility.DeserializeObject("Files/mind.bin");
            Net = new AntibodyNetwork(net);

            return Net;
        }

        //public void AddCellToNetwork(string id string phrase)
        //{
        //    AddTextToNetwork(phrase, id);
        //}

        //public void AddTextToMemory(string id)
        //{
        //    string phrase = Brain.Senses.UserInput;
        //    Extensions.WriteInputToMemory(id, phrase, string.Empty);
        //}

        //public void AddTextToNetwork(string text, string cell)
        //{

        //}

        //private void AddNodeToNetwork(Antibody node)
        //{

        //}

        #endregion
    }
}
