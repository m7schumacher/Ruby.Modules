using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    internal class BCell : Cell
    {
        double effectiveness { get; set; }

        public BCell(List<string> keys) : base()
        {
            base.setKeyWords(keys);
        }

        public BCell(List<string> keys,string id) : base(id)
        {
            base.setKeyWords(keys);
        }

        public virtual double Compute_Effectiveness(string[] words)
        {
            return 0.0;
        }
    }
}
