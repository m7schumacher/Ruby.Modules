//using Ruby.Mind;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Swiss;

//namespace Ruby.Immune.System
//{
//    public class Bot
//    {
//        private Dictionary<string, double> Values { get; set; }
//        private Dictionary<Antibody.Type, double> Bumps { get; set; }

//        public string DeteremineBestWarrior(Antigen invader)
//        {
//            var result = string.Empty;
//            var command = Brain.Experiences.Commands.FirstOrDefault(cmd => cmd.Text.EqualsIgnoreCase(invader.Text));

//            if (command != null)
//            {
//                result = command.ID;
//            }
//            else if (invader.Class != Antigen.Type.Reflex)
//            {
//                IntroduceBias(invader);

//                if (invader.Targeting)
//                {
//                    result = invader.Target;
//                }
//                else
//                {
//                    string[] bits = RefineInput(invader.Text);

//                    if (bits != null)
//                    {
//                        Dictionary<string, double> results = EvaluateInput(bits);
//                        result = AssessResults(results);
//                    }
//                }
//            }

//            //if (String.IsNullOrEmpty(result))
//            //{
//            //    var match = KeyWords.FirstOrDefault(pair => pair.Value.Any(key => invader.Text.Contains(key)));
//            //    result = match.Key ?? string.Empty;
//            //}

//            return result.Trim();
//        }

//        public void IntroduceBias(Antigen invader)
//        {
//            if (invader.Class == Antigen.Type.Question)
//            {
//                Bumps[Antibody.Type.Adjective] = .3;
//                Bumps[Antibody.Type.Preposition] = .2;
//            }
//            else if (invader.Class == Antigen.Type.Command)
//            {
//                Bumps[Antibody.Type.Verb] = .5;
//            }
//        }

//        public string[] RefineInput(string input)
//        {
//            string[] bits = Extensions.TagAndBreak(input);
//            bits = Extensions.FilterStrings(bits);

//            if (bits.Length == 1 && bits[0].Contains("VB"))
//            {
//                return null;
//            }

//            bits = Extensions.StripOfPOS(bits);

//            return bits;
//        }

//        public Dictionary<string, double> EvaluateInput(string[] bits)
//        {
//            Dictionary<string, double> vals = new Dictionary<string, double>();

//            List<string> package = new List<string>();

//            for (int i = 0; i < bits.Length; i++)
//            {
//                string word = bits[i];

//                Antibody twin = Network.GetMatchingNode(word);

//                if (twin != null)
//                {
//                    package = twin.Cells.Union(package).ToList();

//                    foreach (string str in twin.Cells)
//                    {
//                        bool seen = vals.ContainsKey(str);

//                        if (seen)
//                        {
//                            vals[str] += twin.Importance + Bumps[twin.POS];
//                        }
//                        else
//                        {
//                            vals.Add(str, twin.Importance + Bumps[twin.POS]);
//                        }
//                    }
//                }
//            }

//            return vals;
//        }

//        public string AssessResults(Dictionary<string, double> results)
//        {
//            string result = string.Empty;

//            if (results.Count > 0)
//            {
//                var keys = results.GetKeysWithLargestValues();

//                if (keys.Count > 1)
//                {
//                    keys.Remove("169");
//                }

//                result = keys.Count > 1 ? string.Empty : keys.First();
//            }

//            return result;
//        }
//    }

//    public class Optimizer
//    {

//    }
//}
