using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    internal class Memory_BCell : BCell
    {
        string target;
        string spec;

        public Memory_BCell(List<string> keys, string t, string s) : base(keys)
        {
            target = t;
            spec = s;
        }

        public Memory_BCell(List<string> keys, string t) : base(keys)
        {
            target = t;
        }

        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        public string Spec
        {
            get { return spec; }
            set { spec = value; }
        }

        public override double Compute_Effectiveness(string[] words)
        {
            if (words.Length == 1) { return 0; }

            double effectiveness = 0;
            int matches = 0;
            int total = words.Length;

            List<string> keys = getKeyWords();

            foreach(string s in keys)
            {
                if(words.Contains(s))
                {
                    matches++;
                }
            }

            effectiveness = (double)matches / total;

            if(effectiveness == 1)
            {
                int stop = 0;
            }
 
            Effectiveness = effectiveness;
            
            return effectiveness;
        }
    }
}
