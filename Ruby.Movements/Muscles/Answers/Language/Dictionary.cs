using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Swiss;
using Swiss.Web;
using Ruby.Internal;

namespace Ruby.Movements
{
    internal class WordDefiner : Answer
    {
        string API_START = "http://www.dictionaryapi.com/api/v1/references/collegiate/xml/";
        string API_KEY = "?key=45c6bf9a-da60-40d3-8e64-67969caa90f1";

        public WordDefiner()
        {
            Name = "Dictionary";
            Spec = "definition";
            ID = "166";
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = string.Empty;
        }

        public override void GatherValues()
        {
            string search = Core.External.Search;
            string definition = GetDefinition(search);

            definition = definition.Length > 0 ? definition : "I could not find a definition";

            Values = new Dictionary<string, string>()
            {
                { "definition", definition }
            };
        }

        public override bool Stretch()
        {
            string defAthlete = GetDefinition("athlete");
            string defComputer = GetDefinition("computer");
            string defChip = GetDefinition("chip");

            string[] defs = 
            {
                "a person who is trained or skilled in exercises, sports, or games requiring physical strength, agility, or stamina",
                "a programmable usually electronic device that can store, retrieve, and process data",
                "a small wafer of semiconductor material that forms the base for an integrated circuit"
            };

            bool passed = defAthlete.Equals(defs[0]) && defComputer.Equals(defs[1]) && defChip.Equals(defs[2]);

            return passed;
        }

        private string GetDefinition(string search)
        {
            string query = API_START + search + API_KEY;

            string result = InternetUtility.MakeWebRequest(query);

            string[] splitter = Regex.Split(result, "<def>|</def>");
            string[] deffer = Regex.Split(splitter[1], "<dt>:|</dt>");

            if (deffer.Length > 0)
            {
                IEnumerable<string> defs = deffer.Where(d => !d.Contains("<"));

                string definition = defs.OrderByDescending(d => d.Length).First();

                return definition;
            }
            else
            {
                return string.Empty;
            }
        }

        public override void WarmUp() { }
    }
}
