using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    internal class ImmSystem
    {
        internal AntibodyNetwork Antibodies { get; set; }

        public ImmSystem(AntibodyNetwork net)
        {
            Antibodies = net;
        }
    }
}
