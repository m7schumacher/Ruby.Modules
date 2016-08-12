using Swiss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Movements.Muscles
{
    public class Node
    {
        public List<string> Words { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(params string[] args)
        {
            args.ForEach(arg => Words.Add(arg));
        }
    }
}
