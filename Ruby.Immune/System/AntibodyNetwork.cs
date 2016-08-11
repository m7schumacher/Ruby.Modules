using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    [Serializable]
    internal class AntibodyNetwork
    {
        public ConcurrentBag<Antibody> Antibodies { get; set; }
        
        public IEnumerable<Antibody> Nouns { get { return Antibodies.Where(nd => nd.POS == Antibody.Type.Noun); } }
        public IEnumerable<Antibody> Verbs { get { return Antibodies.Where(nd => nd.POS == Antibody.Type.Verb); } }
        public IEnumerable<Antibody> Adjectives { get { return Antibodies.Where(nd => nd.POS == Antibody.Type.Adjective); } }

        public AntibodyNetwork(ConcurrentBag<Antibody> b)
        {
            Antibodies = b;
        }

        public AntibodyNetwork()
        {
            Antibodies = new ConcurrentBag<Antibody>();
        }

        public bool SeenBefore(Antibody child)
        {
            return Antibodies.Contains(child);
        }

        public Antibody GetMatchingNode(Antibody node)
        {
            return Antibodies.FirstOrDefault(nd => nd.Equals(node));
        }

        public Antibody GetMatchingNode(string text)
        {
            return Antibodies.FirstOrDefault(nd => nd.Text.Equals(text));
        }

        public void AddNode(Antibody nd)
        {
            Antibodies.Add(nd);
        }

        public void OutputNetwork(string file)
        {
            string[] lines = new string[Antibodies.Count];

            for(int i = 0; i < Antibodies.Count; i++)
            {
                Antibody nd = Antibodies.ElementAt(i);

                string line = nd.Text + " -> ";

                foreach(Antibody child in nd.Children)
                {
                    line += child.Text + "|";
                }

                line += " [ ";

                nd.Cells.ToList().ForEach(cell => line += cell + " ");

                line += "]";

                lines[i] = line;
            }

            File.WriteAllLines(file, lines);
        }

        public void WriteNetwork(string file)
        {
            Swiss.FileUtility.SerializeObject(Antibodies, file);
        }
    }
}
