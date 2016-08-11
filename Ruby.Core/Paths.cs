using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Atlas.Internal
{
    public class Paths
    {
        public string Core { get; set; }
        public string Memory { get; set; }
        public string Cells { get; set; }
        public string Keys { get; set; }
        public string Words { get; set; }
        public string Recents { get; set; }
        public string Vocabulary { get; set; }
        
        public string Secrets { get; set; }
        public string Processes { get; set; }

        public string SettingsFile { get; set; }
        public string TestMemory { get; set; }

        public string GoogleURL { get; set; }
        public string GoogleAPIKey { get; set; }
        public string GoogleAPISecret { get; set; }
        public string GoogleAPIClient { get; set; }
        public string GoogleUsername { get; set; }
        public string GooglePassword { get; set; }

        public string SpotifyUserID { get; set; }
        public string SpotifyClientID { get; set; }
        public string SpofityClientSecret { get; set; }

        public string FitbitConsumerKey { get; set; }
        public string FitbitConsumerSecret { get; set; }

        public string PushbulletKey { get; set; }

        public string WolframKey { get; set; }
        public string WeatherKey { get; set; }
        public string BibleBooks { get; set; }

        public string Songs { get; set; }
        public string Song_Keys { get; set; }

        public string BaseHostURL { get { return "http://192.168.1.18:49560/api/Command/Execute?prompt="; } }

        public Paths() : base()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            Core = @"C:\Users\mschuee\Documents\Visual Studio 2013\Projects\Atlas\Atlas\";

            Memory = "Files/memory.txt";
            TestMemory = "Files/test_memory.txt";
            Cells = "Files/cells.txt";
            Keys = "Files/keys.txt";
            Words = "Files/words.txt";
            Recents = "Files/Recent.txt";
            Vocabulary = "Files/vocabulary.txt";
            SettingsFile = "Files/Settings.txt";
            Processes = "Files/Processes.txt";
            Secrets = "Files/secrets.txt";
            Song_Keys = "Files/SpotifyKeys.txt";
            Songs = "Files/Songs.txt";

            GatherKeysAndSecrets();
        }

        private void GatherKeysAndSecrets()
        {
            string[] secrets = File.ReadAllLines(Secrets);

            SpotifyUserID = GetLineValue("spotify_username", secrets);
            SpotifyClientID = GetLineValue("spotify_client_id", secrets);
            SpofityClientSecret = GetLineValue("spotify_secret", secrets);

            GoogleAPIClient = GetLineValue("google_client_id", secrets);
            GoogleAPIKey = GetLineValue("google_key", secrets);
            GoogleAPISecret = GetLineValue("google_secret", secrets);
            GoogleUsername = GetLineValue("google_username", secrets);
            GooglePassword = GetLineValue("google_password", secrets);
            FitbitConsumerKey = GetLineValue("fitbit_consumer_key", secrets);
            FitbitConsumerSecret = GetLineValue("fitbit_consumer_secret", secrets);
            PushbulletKey = GetLineValue("pushbullet_key", secrets);
            WeatherKey = GetLineValue("weather_key", secrets);

            WolframKey = GetLineValue("wolfram", secrets);
        }

        private string GetLineValue(string target, string[] lines)
        {
            return lines.First(line => line.StartsWith(target)).Split('=')[1];
        }
    }
}
