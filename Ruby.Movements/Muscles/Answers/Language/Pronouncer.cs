using Atlas.Mind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Muscle
{
    internal class Pronouncer : Reflex
    {
        public Pronouncer()
        {
            Name = "Pronouncer";
            ID = "200";
        }

        public override void EstablishRecognizers()
        {
            Recognizers = new Dictionary<string, string[]>()
            {
                { "prounounce ||", new string[] { } },
            };
        }

        public override string Execute()
        {
            string phrase = Brain.Senses.Search;
            return phrase;
        }
    }
}
