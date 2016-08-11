using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Swiss;
using Ruby.Internal;

namespace Ruby.Immune
{
    internal class Reader
    {
        private Dictionary<string, List<string>> KeyWords { get; set; }

        private Dictionary<string, double> Values { get; set; }
        private Dictionary<Antibody.Type, double> Bumps { get; set; }
        private Dictionary<string, int> Counts;

        private AntibodyNetwork Network { get { return ImmuneGate.Network; } }

        public Reader()
        {
            Counts = new Dictionary<string, int>();
            Values = new Dictionary<string, double>();
            KeyWords = new Dictionary<string, List<string>>();

            Bumps = new Dictionary<Antibody.Type, double>()
            {
                { Antibody.Type.Noun, 0 },
                { Antibody.Type.Adjective, 0 },
                { Antibody.Type.Verb, 0 },
                { Antibody.Type.Questioner, 0 },
                { Antibody.Type.Preposition, 0 },
                { Antibody.Type.None, 0 }
            };

            foreach (string line in File.ReadAllLines(Core.FilePaths.Cells))
            {
                string[] splitter = line.Split('|');

                if (splitter.Length == 4)
                {
                    string id = splitter[1];
                    string keys = splitter[2];
                    string target = splitter[3];

                    if (keys.Length > 0)
                    {
                        KeyWords.Add(id, keys.Split(' ').Select(bit => " " + bit + " ").ToList());
                    }
                }
            }
        }

        public string DeteremineBestWarrior(Antigen invader)
        {
            var result = string.Empty;

            if(invader.Class != Antigen.Type.Reflex)
            {
                IntroduceBias(invader);

                if(invader.IsTargeting)
                {
                    result = invader.Target;
                }
                else
                {
                    string[] bits = RefineInput(invader.Text);

                    if(bits != null)
                    {
                        Dictionary<string, double> results = EvaluateInput(bits);
                        result = AssessResults(results);
                    }
                } 
            }

            if(String.IsNullOrEmpty(result))
            {
                var match = KeyWords.FirstOrDefault(pair => pair.Value.Any(key => invader.Text.Contains(key)));
                result = match.Key ?? string.Empty;
            }

            return result.Trim();
        }

        public void IntroduceBias(Antigen invader)
        {
            if (invader.Class == Antigen.Type.Question)
            {
                Bumps[Antibody.Type.Adjective] = .3;
                Bumps[Antibody.Type.Preposition] = .2;
            }
            else if (invader.Class == Antigen.Type.Command)
            {
                Bumps[Antibody.Type.Verb] = .5;
            }
        }

        public string[] RefineInput(string input)
        {
            input = Extensions.FilterUselessStrings(input);

            if(!String.IsNullOrEmpty(input))
            {
                string[] bits = Extensions.TagAndBreak(input);
                bits = Extensions.FilterOnPOS(bits);

                if (bits.Length == 1 && bits[0].Contains("VB"))
                {
                    return null;
                }

                bits = Extensions.StripOfPOS(bits);

                return bits;
            }

            return null;
        }

        public Dictionary<string, double> EvaluateInput(string[] bits)
        {
            Dictionary<string, double> vals = new Dictionary<string, double>();

            foreach(string word in bits)
            {
                Antibody twin = Network.GetMatchingNode(word);

                if (twin != null)
                {
                    foreach(var cell in twin.Cells)
                    {
                        vals.IncrementOrAddNew(cell, twin.Importance + Bumps[twin.POS]);
                    }
                }
            }

            return vals;
        }

        public string AssessResults(Dictionary<string, double> results)
        {
            string result = string.Empty;

            if (results.Count > 0)
            {
                var keys = results.GetKeysWithLargestValues();

                if(keys.Count > 1)
                {
                    keys.Remove("169");
                }

                result = keys.Count > 1 ? string.Empty : keys.First();
            }

            return result;
        }
    }
}
