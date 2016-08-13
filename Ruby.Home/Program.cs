using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruby.Mind;
using Ruby.Internal;

namespace Atlas.Home
{
    class Program
    {
        static void Main(string[] args)
        {
            Brain.FireUp();
            Console.WriteLine("Initializing...");

            Task readier = Task.Factory.StartNew(() =>
            {
                Brain.Awaken(true);
            });

            while (Core.Internal.State != Hypothalmus.States.Ready) { }

            while (true)
            {
                string input = Console.ReadLine();
                Brain.Process(input);
            }
        }
    }
}
