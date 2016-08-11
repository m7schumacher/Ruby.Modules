//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace Ruby.Immune
//{
//    [Serializable]
//    public class Killer_TCell : TCell 
//    {
//        string target;
//        bool website = false;

//        public Killer_TCell(List<string> keys, string t, string id) : base(keys,id)
//        {
//            target = t;
//            website = t.Contains("http");
//        }

//        public Killer_TCell(List<string> keys, string t, string id, int key) : base(keys, id, key)
//        {
//            target = t;
//            website = t.Contains("http");
//        }

//        public Killer_TCell(List<string> keys, string t) : base(keys)
//        {
//            target = t;
//            website = t.Contains("http");
//        }

//        public string Target
//        {
//            get { return target; }
//            set { target = value; }
//        }

//        public bool Website
//        {
//            get { return website; }
//        }

       
//    }
//}
