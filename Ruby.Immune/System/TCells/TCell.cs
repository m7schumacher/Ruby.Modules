//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Ruby.Immune
//{
//    [Serializable]
//    public class TCell : Cell
//    {
//        double effectiveness { get; set; }

//        public TCell(List<string> keys) : base()
//        {
//            base.setKeyWords(keys);
//        }

//        public TCell(List<string> keys,string id) : base(id)
//        {
//            base.setKeyWords(keys);
//        }

//        public TCell(List<string> keys, string id, int key) : base(id, key)
//        {
//            base.setKeyWords(keys);
//        }

//        public bool IsEffective(string[] words)
//        {
//            return KeyWords.Any(key => words.Contains(key));
//        }

//        public virtual void Execute()
//        {

//        }
//    }
//}