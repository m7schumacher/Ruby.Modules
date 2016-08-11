using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Swiss;

namespace Ruby.Immune
{
    public class Antigen
    {
        public enum Type
        {
            Command,
            Question,
            Reflex
        }

        public Type Class { get; set; }

        public string Text { get; set; }
        public string[] Bits { get; set; }

        public bool IsSearching { get; set; }
        public string Search { get; set; }

        public bool IsTargeting { get; set; }
        public string Target { get; set; }

        public string Spec { get; set; }

        public Antigen(string input)
        {
            input = input.ToLower();
            input = input.Replace("\"","'");
            input = input.Remove("'s");

            Regex questioners = new Regex("^who|^is|^what|^when|^where|^why|^how|[?]");
            Regex searchers = new Regex("'.+'");
            Regex targeters = new Regex(@"\[.+\]");

            input = Regex.Replace(input, "[?]", "");
            input = Regex.Replace(input, @"\s+i\s+", " I ");
            input = Regex.Replace(input, "i$", "I");

            bool isQuestion = questioners.IsMatch(input);
            bool isSearch = searchers.IsMatch(input);
            bool isTarget = targeters.IsMatch(input);

            if(isQuestion)
            {
                Class = Type.Question;
            }

            if(isSearch)
            {
                IsSearching = true;
                Search = searchers.Match(input).Value.Remove("'");
            }

            if(isTarget)
            {
                IsTargeting = true;
                Target = targeters.Match(input).Value.TrimCharactersWithin(new char[] { '[', ']' });
            }

            string withoutQuotes = Regex.Replace(input, "'.+'", "par");
            withoutQuotes = Regex.Replace(withoutQuotes, @"\[.+\]", "");
            withoutQuotes = withoutQuotes.Trim();

            string[] bits = withoutQuotes.SplitOnWhiteSpace();

            if(bits.Length == 2 && bits[1].Equals("par"))
            {
                withoutQuotes = bits[0];
                Class = Type.Reflex;
            }

            Text = withoutQuotes;
            Bits = bits;
        }
    }
}
