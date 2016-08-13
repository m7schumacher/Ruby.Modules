using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Movements.Muscles
{
    internal class Vocabulary
    {
        public List<Phrase> Phrases { get; set; }

        public Vocabulary(List<Phrase> phrases)
        {
            Phrases = phrases;
        }
    }
}
