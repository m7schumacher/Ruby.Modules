using Ruby.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ruby.Movements
{
    internal class FantasyFootballNews : Answer
    {
        private string[] MyTeam = new string[]
        {
            "sam bradford",
            "doug martin",
            "justin forsett",
            "julio jones",
            "odell beckham",
            "mike evans",
            "jordan cameron",
            "isaiah crowell",
            "andre ellington",
            "stafford",
            "charles johnson",
            "amari cooper",
            "zach ertz",
            "latavious murray"
        };

        private Dictionary<string, string> positions = new Dictionary<string, string>()
        {
            {" qb ", " quarterback "},
            {" rb ", " running back "},
            {" wr ", " wide receiver "},
            {" te ", " tight end "},
            {" k ", " kicker "},
            {" d/st ", " defense "}
        };

        List<string> stories;

        public FantasyFootballNews() : base()
        {
            Name = "FantasyFootball";
            ID = "167";
            Key = "ffb";

            stories = new List<string>();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = "random";
        }

        public override void GenerateRecognizedPhrases()
        {
            RecognizedPhrases = new Dictionary<string, string[]>()
            {
                { "any news on {}", MyTeam },
            };
        }

        public override void Initialize()
        {
            string url = "http://www.rotoworld.com/rss/feed.aspx?sport=nfl&ftype=news&count=50&format=rss";

            XmlDocument doc = new XmlDocument();
            doc.Load(url);

            foreach (XmlNode node in doc.SelectNodes("rss/channel/item"))
            {
                string text = node.SelectSingleNode("description").InnerText;

                text = text.Replace("&quot;", "");
                text = text.ToLower();

                foreach (string str in positions.Keys)
                {
                    text = text.Replace(str, positions[str]);
                }

                stories.Add(text);
            }
        }

        private string TellTwoRandomStories()
        {
            Random rand = new Random();
            int randomOne = rand.Next(stories.Count);
            int randomTwo = randomOne;

            while (randomTwo == randomOne)
            {
                randomTwo = rand.Next(stories.Count);
            }

            return stories.ElementAt(randomOne) + " and secondly, " + stories.ElementAt(randomTwo);
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>()
            {
                { "team", new string[] { "my team", "my fantasy team", "am I doing in fantasy" } },
            };
        }

        public override void SetSecondarySpecs() 
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        public override void GatherValues()
        {
            Values = new Dictionary<string, string>();

            string input = Core.External.UserInput;

            var pressingStories = stories
                .Where(story => MyTeam
                                .Any(player => story.Contains(player)));

            bool pressing = pressingStories.Count() > 0;

            if (Spec.Equals("team"))
            {
                if (pressing)
                {
                    int numberOfPressingStories = pressingStories.Count();

                    if (numberOfPressingStories > 1)
                    {
                        Spec += "_multi";

                        Values.Add("first", pressingStories.ElementAt(0));
                        Values.Add("second", pressingStories.ElementAt(1));
                    }
                    else
                    {
                        Values.Add("first", pressingStories.First());
                    }
                }
                else
                {
                    Spec += "_none";
                }
            }
            else if (!String.IsNullOrEmpty(Core.External.Search))
            {
                string player = Core.External.Search;
                string story = stories.FirstOrDefault(str => str.Contains(player));

                Spec += "_player";
                Values.Add("player", player);

                if(story != null)
                {
                    Values.Add("story", story);
                }
                else
                {
                    Spec += "_none";
                }
            }
            else
            {
                Values.Add("random", TellTwoRandomStories());
            }
        } 
    }
}
