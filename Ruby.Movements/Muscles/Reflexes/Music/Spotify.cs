using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Windows.Forms;
using System.Timers;
using SpotifyAPI.Web;
using Ruby.Muscle.Music;
using Swiss;
using Ruby.Internal;

namespace Ruby.Muscle
{
    internal class Spotify : Reflex
    {
        Dictionary<string[], string> categories = new Dictionary<string[], string>()
        {
            { new string[]{ "christmas","holidays","snow" }, "Snow" },
            { new string[]{ "hillsong" }, "Hillsong" },
            { new string[]{ "bethel" }, "Bethel" },
            { new string[]{ "pump", "tempo", "beats","workout" }, "Tempo" },
            { new string[]{ "rock", "metal", "heavy" }, "Rock" },
            { new string[]{ "techno", "dance", "tech", "dancing" }, "Tech"},
            { new string[]{ "worship", "praise"}, "Worship" },
            { new string[]{ "christian", "cross", "church" }, "Cross" },
            { new string[]{ "quiet time", "peaceful", "wind down", "unwind" }, "Peace" },
            { new string[]{ "Alpha" }, "Alpha" },
            { new string[]{ "jam", "smooth", "jam out" }, "Jam" },
            { new string[]{ "piano", "classical" }, "Piano" },
            { new string[]{ "lets chill", "chill", "chill out" }, "Chill" },
            { new string[]{ "love songs", "animus", "boo" }, "Boo" },
            { new string[]{ "classic", "some classics", "oldies", "high school" }, "Classic" },
            { new string[]{ "shazam" }, "Shazam" },
            { new string[]{ "lets party", "party", "take shots", "shot o'clock" }, "Party" },
            { new string[]{ "study", "relax", "gravity", "program" }, "Gravity" },
            { new string[]{ "call of duty", "walking"}, "Walk" }
        };

        internal LocalAPI Local;
        internal WebAPI Web; 

        string USER_ID, CLIENT_ID, SECRET;
        string ACCESS_TOKEN, REFRESH_TOKEN;
        string KEYS_FILE, SONGS_FILE;

        public Spotify() : base() 
        {
            USER_ID = Core.FilePaths.SpotifyUserID;
            CLIENT_ID = Core.FilePaths.SpotifyClientID;
            SECRET = Core.FilePaths.SpofityClientSecret;
            KEYS_FILE = Core.FilePaths.Song_Keys;
            SONGS_FILE = Core.FilePaths.Songs;
            ACCESS_TOKEN = string.Empty;
            REFRESH_TOKEN = string.Empty;

            Name = "Music";
            ID = "151";
        }

        public override void Initialize()
        {
            Web = new WebAPI(CLIENT_ID, KEYS_FILE);
            Local = new LocalAPI();

            foreach(var play in Web.Playlists)
            {
                categories.Add(new string[] {}, play.Name);
            }

            Core.Internal.IsPlayingSpotify = Local.IsPlaying();
            Core.Internal.IsConnectedToSpotify = true;
        }

        public override void EstablishRecognizers()
        {
            List<string> specs = new List<string>();
            categories.Keys.ToList().ForEach(key => specs.AddRange(key));

            Recognizers = new Dictionary<string, string[]>()
            {
                { "{} the song", new string[] { "resume", "play", "pause", "stop" } },
                { "play some || music", specs.ToArray() },
            };
        }

        public override string Execute()
        {
            if (Core.Internal.isQuiet)
            {
                return "I am in quiet mode sir";
            }

            string input = Core.External.UserInput;

            string[] skippers = new string[] { "next", "skip" };
            string[] players = new string[] { "keep playing", "resume" };
            string[] stoppers = new string[] { "stop", "go quiet", "pause" };
            string[] questions = new string[] { "what song" };

            string response = string.Empty;

            if(input.Equals("update songs"))
            {
                Web.UpdateSongs();
                return "Songs have been updated sir";
            }
            else if(!String.IsNullOrEmpty(Core.External.Search))
            {
                string name = Core.External.Search;
                Play(name);
            }
            else if(players.Any(str => input.Contains(str)))
            {
                Local.Play();
                Core.Internal.IsPlayingSpotify = true;
            }
            else if (skippers.Any(str => input.Contains(str)))
            {
                Local.Next();
            }
            else if (stoppers.Any(str => input.Contains(str)))
            {
                Local.Pause();
                Core.Internal.IsPlayingSpotify = false;
            }
            else if (questions.Any(str => input.Contains(str)))
            {
                response = Local.CurrentSongString();
            }
            else
            {
                string[] key = categories.Keys.FirstOrDefault(ks => ks.Any(k => input.Contains(k)));

                if (key != null)
                {
                    Local.Pause();
                    response = "Now playing your" + categories[key] + " playlist";
                    Play(categories[key].ToLower());
                }
                else { response = "I am unsure what to play"; }
            }

            return response;
        }

        public void Play(string str)
        {
            str = str.ToLower();
            string key = string.Empty;

            string uri = Web.GetPlaylistURIFromName(str);

            if (uri.IsEmpty())
            {
                uri = Web.GetTrackURIFromName(str);
            }

            if (!uri.IsEmpty())
            {
                Local.Play(uri);
            }
            else
            {
                Swiss.SpeechUtility.SpeakText("I'm not seeing that song sir", Core.Internal.isQuiet);
            }
        }
    }
}
