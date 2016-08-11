//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using System.Reflection;

//namespace Ruby.Immune
//{
//    public class Lymphocyte : TCell
//    {
//        string method;

//        public Lymphocyte(List<string> keys, string t, string id) : base(keys,id)
//        {
//            method = t;
//        }

//        public Lymphocyte(List<string> keys, string t, string id, int key) : base(keys, id, key)
//        {
//            method = t;
//        }

//        public Lymphocyte(List<string> keys, string t) : base(keys)
//        {
//            method = t;
//        }

//        public string Target
//        {
//            get { return method; }
//            set { method = value; }
//        }

//        public override void Execute()
//        {
//            //Type thisType = animator.GetType();
//            //MethodInfo theMethod = thisType.GetMethod(method);
//            //theMethod.Invoke(this, new object[]{});
//        }
//    }

//}
