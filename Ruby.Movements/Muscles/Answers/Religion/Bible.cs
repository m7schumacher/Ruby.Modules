using Ruby.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ruby.Movements
{
    internal class Bible : Answer
    {
        private string url = "http://labs.bible.org/api/?passage=";

        public Bible() : base()
        {
            Name = "Bible";
            Key = "bible";
            ID = "155";
        }

        public override void Initialize()
        {
            KeyWords = File.ReadAllLines(Core.FilePaths.BibleBooks).Select(line => line.ToLower()).ToArray();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = "verse";
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        private string ExtractVerses(string input)
        {
            Regex verseFinder = new Regex(@"\d:^\s");

            Match mtch = verseFinder.Match(input);

            return string.Empty;
        }

        public override void GatherValues()
        {
            ExtractVerses(Core.External.UserInput);

            string verses = Core.External.Search.Trim().Replace(" ", "+");
            string searchURL = url + verses + "&type=json";

            WebClient client = new WebClient();
            string json = client.DownloadString(searchURL).Replace("[", "").Replace("]", "");

            string book = string.Empty;
            string chapter = string.Empty;
            string number = null;
            string verse = string.Empty;

            string[] splitter = json.Split('{');

            for (int i = 1; i < splitter.Length; i++)
            {
                int end = i == splitter.Length - 1 ? splitter[i].Length : splitter[i].Length - 1;
                string srch = "{" + splitter[i].Substring(0, end);
                JToken token = JObject.Parse(srch);

                if(i == splitter.Length - 1 && splitter.Length > 2)
                {
                    number += " through " + token.SelectToken("verse").ToString();
                }
                else
                {
                    number = number ?? token.SelectToken("verse").ToString();
                    chapter = token.SelectToken("chapter").ToString();
                    book = token.SelectToken("bookname").ToString();
                }

                verse += token.SelectToken("text").ToString() + " ";
            }

            Values = new Dictionary<string, string>()
            {
                { "book", book },
                { "chapter", chapter },
                { "number", number },
                { "verse", verse }
            };
        }
    }
}
