//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using System.Reflection;
//using System.Windows;

//namespace Ruby.Immune
//{
//    public class Helper_TCell : TCell
//    {
//        string method;

//        public Helper_TCell(List<string> keys, string t, string id) : base(keys,id)
//        {
//            method = t;
//        }

//        public Helper_TCell(List<string> keys, string t, string id, int key) : base(keys, id, key)
//        {
//            method = t;
//        }

//        public Helper_TCell(List<string> keys, string t) : base(keys)
//        {
//            method = t;
//        }

//        public string Target
//        {
//            get { return method; }
//            set { method = value; }
//        }
//    }
//}
