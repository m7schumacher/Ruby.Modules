using Google.Apis.Drive.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby{
    internal class DriveElement
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string ParentID { get; set; }

        public DriveElement(string id, string nm)
        {
            ID = id;
            Title = nm;
        }

        public DriveElement(string id, string nm, IEnumerable<ParentReference> parents)
        {
            ID = id;
            Title = nm;

            ParentID = parents.Count() > 0 ? parents.First().Id : "rt";
        }
    }
}
