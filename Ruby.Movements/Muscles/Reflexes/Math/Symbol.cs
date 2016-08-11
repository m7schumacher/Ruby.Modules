using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Muscle.Muscles.Muscles.Math
{
    public class Symbol
    {

        public string Sym { get; set; }

        public Symbol leftChild { get; set; }
        public Symbol rightChild { get; set; }

        public Symbol(string sym)
        {
            Sym = sym;
        }

        public void AddLeft(Symbol sym)
        {
            leftChild = sym;
        }

        public void AddRight(Symbol sym)
        {
            rightChild = sym;
        }
    }
}
