using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    internal class StemCell : Cell
    {
        string type;

        public StemCell(string t) : base()
        {
            type = t;
        }

        public void Specialize()
        {

        }
    }
}
