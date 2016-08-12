using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Swiss;
using Swiss.Web;
using Ruby.Internal;

namespace Ruby.Movements
{
    internal class SetOfSkills
    {
        public List<Response> Responses { get; set; }

        #region Movements

        public Calendar Calendar { get; set; }
        public Spotify Spotify { get; set; }
        public GMaps Map { get; set; }
        public Weather Weather { get; set; }
        public Yelper Yelp { get; set; }
        public FileLocator Locate { get; set; }
        public Googler Google { get; set; }
        public Calculator Math { get; set; }
        public Terminator Terminate { get; set; }
        public Bible Bible { get; set; }
        public Scores Scores { get; set; }
        public TCPListener TCP { get; set; }
        public WordDefiner Dictionary { get; set; }
        //public Fitbit Fitbit { get; set; }
        public FantasyFootballNews FantasyNews { get; set; }
        public SmallTalk SmallTalk { get; set; }
        public Wolf Wolfram { get; set; }
        public Configurer Configurator { get; set; }
        public Pronouncer Pronouncer { get; set; }

        #endregion

        private Dictionary<string, List<string>> KeyWords { get; set; }

        public SetOfSkills()
        {
            Responses = new List<Response>();
            KeyWords = new Dictionary<string, List<string>>();
  
            GatherMovements();
            GatherReflexes();
        }

        public Response GetResponseByID(string id)
        {
            string input = Core.External.UserInput;

            if(String.IsNullOrEmpty(id))
            {
                var match = KeyWords.FirstOrDefault(pair => pair.Value.Any(key => input.Contains(key)));
                id = match.Key;
            }

            return Responses.FirstOrDefault(resp => resp.ID.Equals(id));
        }

        public Response GetResponseByName(string name)
        {
            return Responses.FirstOrDefault(resp => resp.Name.Equals(name));
        }

        private void GatherReflexes()
        {
            string[] lines = File.ReadAllLines(Core.FilePaths.Cells);
            string[] splitter;

            foreach (string line in lines)
            {
                splitter = line.Split('|');

                if (splitter.Length == 4)
                {
                    string id = splitter[1];
                    string keys = splitter[2];
                    string target = splitter[3];

                    if(keys.Length > 0)
                    {
                        string[] splitKeys = keys.Split(' ');
                        KeyWords.Add(id, splitKeys.ToList());
                    }
                    
                    if (!target.StartsWith("-"))
                    {
                        Reflex reflex = new Reflex(target);
                        reflex.ID = id;

                        Responses.Add(reflex);
                    }
                }
            }
        }

        private void GatherMovements()
        {
            Calendar = new Calendar();
            Spotify = new Spotify();
            Map = new GMaps();
            Weather = new Weather();
            Yelp = new Yelper();
            Locate = new FileLocator();
            Google = new Googler();
            Math = new Calculator();
            Terminate = new Terminator();
            Bible = new Bible();
            Scores = new Scores();
            Calendar = new Calendar();
            TCP = new TCPListener();
            Dictionary = new WordDefiner();
            //Fitbit = new Fitbit();
            FantasyNews = new FantasyFootballNews();
            SmallTalk = new SmallTalk();
            Wolfram = new Wolf();
            Configurator = new Configurer();
            Pronouncer = new Pronouncer();

            InitializeTools();
        }

        private void InitializeTools()
        {
            //if (!InternetUtility.CheckForInternetConnection())
            //{
            //    Terminate.Initialize();
            //    return;
            //}

            Response[] functions = new Response[] 
            { 
                Spotify, Map, Yelp, Locate, Weather, Google, Math, Terminate,
                Scores, Calendar, TCP, Dictionary, FantasyNews, SmallTalk, Wolfram,
                Configurator, Pronouncer
            };

            Task initializer = Task.Factory.StartNew(() =>
            {
                foreach (Response tool in functions)
                {
                    try
                    {
                        tool.Initialize();
                        Console.WriteLine(tool.Name + " response has been initialized");

                        if(tool.Enabled)
                        {
                            Responses.Add(tool);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(tool.Name + " was unable to initialize\n\n\n" + e.Message + "\n\n\n" + e.StackTrace);
                    }

                }

                GatherKeyWords();

                Core.Internal.State = Hypothalmus.States.Warm;
            });
        }

        public void GatherKeyWords()
        {
            foreach (Response resp in Responses)
            {
                string id = resp.ID;

                if (KeyWords.ContainsKey(id))
                {
                    KeyWords[id].AddRange(resp.KeyWords);
                }
                else
                {
                    KeyWords.Add(id, resp.KeyWords.ToList());
                }
            }
        }
    }
}
