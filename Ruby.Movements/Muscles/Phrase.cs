using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Movements.Muscles
{
    internal class Phrase
    {
        private string Delimeter { get; set; }

        public string Verbage { get; set; }
        public List<string> Blocks { get; set; }

        public Phrase(string verbage, string delimeter = "||")
        {
            Verbage = verbage;
        }

        public void AddBlocks(params string[] args)
        {
            Blocks.AddRange(args);
        }

        public List<string> GeneratePhrases()
        {
            List<string> phrases = Blocks.Select(block => Verbage.Replace(Delimeter, block))
                                         .AllLower()
                                         .ToList();

            return phrases;
        }
    }
}
