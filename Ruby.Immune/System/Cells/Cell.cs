using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    [Serializable]
    public class Cell
    {
        static string global_id;
        static List<string> past_ids = new List<string>();

        public List<string> KeyWords { get; set; }
        public string Local_ID { get; set; }
        public int Key { get; set; }
        public string Phrase { get; set; }
        public double Effectiveness { get; set; }
        public string Spec { get; set; }

        public string Name { get; set; }

        public Cell()
        {
            setID();

            Key = -1;
            Phrase = string.Empty;
            Effectiveness = -1;
            Spec = string.Empty;
        }

        public Cell(string id)
        {
            setID(id);

            Key = -1;
            Phrase = string.Empty;
            Effectiveness = -1;
            Spec = string.Empty;
        }

        public Cell(string id, int key)
        {
            setID(id);
            setKey(key);

            Phrase = string.Empty;
            Effectiveness = -1;
            Spec = string.Empty;
        }

        public void setID()
        {
            int curr = Convert.ToInt16(global_id);
            curr++;

            while(past_ids.Contains(curr.ToString()))
            {
                curr++;
            }

            string new_id = curr.ToString();

            Local_ID = new_id;
            global_id = new_id;
            past_ids.Add(new_id);
        }

        public void setID(string id)
        {
            Local_ID = id;
            past_ids.Add(id);
        }

        public string getID()
        {
            return Local_ID;
        }

        public List<string> getKeyWords()
        {
            return KeyWords;
        }

        public void setKeyWords(List<string> next)
        {
            KeyWords = next;
            string res = string.Empty;

            foreach(string s in next)
            {
                res += s + " ";
            }

            Phrase = res.TrimEnd();
           // Ruby.Framework.Utility.Speech.SpeakTexter.Phrases.Add(Phrase);
        }

        public int getKey()
        {
            return Key;
        }

        public void setKey(int stt)
        {
            Key = stt;
        }

        public void addKeyWord(string word)
        {
            KeyWords.Add(word);
        }

        public virtual string toString()
        {
            return null;
        }
    }
}
