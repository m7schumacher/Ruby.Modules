using Ruby.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Muscle
{
    internal abstract class Response
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string[] KeyWords { get; set; }

        public bool Enabled { get; set; }

        public Dictionary<string, string[]> Recognizers { get; protected set; }

        public Response()
        {
            ID = string.Empty;
            Name = string.Empty;
            Enabled = true;

            KeyWords = new string[] { };

            EstablishRecognizers();
        }

        public virtual void Initialize() { }

        protected void Enable() { Enabled = true; }
        protected void Disable() { Enabled = false; }

        public abstract string Execute();
        public abstract void EstablishRecognizers();
    }
}
