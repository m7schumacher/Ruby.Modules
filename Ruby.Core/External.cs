using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Internal
{
    public class External
    {
        public string Search { get; set; }
        public string UserInput { get; set; }

        public External()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            Search = string.Empty;
            UserInput = string.Empty;
        }
    }
}
