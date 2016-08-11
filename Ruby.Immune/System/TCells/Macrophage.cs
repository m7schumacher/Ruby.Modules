//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace Ruby.Immune
//{
//    public class Macrophage : TCell
//    {
//        List<string> targets;

//        public Macrophage(List<string> keys, List<string> targets, string id) : base(keys,id, 0)
//        {
//            this.targets = targets;
//        }

//        public Macrophage(List<string> keys, List<string> targets) : base(keys)
//        {
//            this.targets = targets;
//        }

//        public List<string> Target
//        {
//            get { return targets; }
//            set { targets = value; }
//        }

//        public override void Execute()
//        {
//            foreach(string s in targets)
//            {
//                if(!Utility.Processes.IsProcessRunning(s))
//                {
//                    Process firstProc = new Process();
//                    firstProc.StartInfo.FileName = s;
//                    firstProc.Start();
//                }
//            }
//        }
//    }
//}
