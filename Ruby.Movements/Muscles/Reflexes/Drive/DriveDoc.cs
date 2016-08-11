using Google.Apis.Drive.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby{
    internal class DriveDoc : DriveElement
    {
        public DriveDoc(string id, string nm) : base(id, nm) 
        { 
        
        }

        public DriveDoc(string id, string nm, IEnumerable<ParentReference> par) : base(id, nm, par)
        {

        }
    }
}
