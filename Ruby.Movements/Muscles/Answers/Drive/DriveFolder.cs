using Google.Apis.Drive.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas{
    internal class DriveFolder : DriveElement
    {
        public List<DriveElement> Children { get; set; }

        public DriveFolder(string id, string nm) : base(id, nm) 
        {
            Children = new List<DriveElement>();
        }

        public DriveFolder(string id, string nm, IEnumerable<ParentReference> par) : base(id, nm, par)
        {
            Children = new List<DriveElement>();
        }
    }
}
