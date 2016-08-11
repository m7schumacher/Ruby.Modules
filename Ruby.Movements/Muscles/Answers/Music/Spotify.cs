using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.SpotifyWebAPI;
using SpotifyAPI.SpotifyWebAPI.Models;
using SpotifyAPI.SpotifyLocalAPI;
using System.Threading;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Windows.Forms;
using Atlas.Mind;
using System.Timers;

namespace Atlas.Muscle
{
    internal class Spotify : Reflex
    {
        private static System.Timers.Timer _nexter;

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

        public Dictionary<string, string> Playlists { get; set; }
        public Dictionary<string, string> Songs { get; set; }
        private List<string> SongsPlayed { get; set; }

        FullPlaylist playlist_current;
        FullTrack song_current;

        SpotifyLocalAPIClass localAPI;
        SpotifyWebAPIClass webAPI;
        SpotifyMusicHandler musicHandler;

        static AutorizationCodeAuth authorizer;
        JsonSerializerSettings settings;

        string USER_ID, CLIENT_ID, SECRET;
        string ACCESS_TOKEN, REFRESH_TOKEN;
        string KEYS_FILE, SONGS_FILE;

        bool wait, useAuth;
        int current_index;

        public Spotify() : base() 
        {
            USER_ID = Brain.FilePaths.SpotifyUserID;
            CLIENT_ID = Brain.FilePaths.SpotifyClientID;
            SECRET = Brain.FilePaths.SpofityClientSecret;
            KEYS_FILE = Brain.FilePaths.Song_Keys;
            SONGS_FILE = Brain.FilePaths.Songs;
            ACCESS_TOKEN = string.Empty;
            REFRESH_TOKEN = string.Empty;

            settings = new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore, 
                TypeNameHandling = TypeNameHandling.All 
            };

            Playlists = new Dictionary<string, string>();
            Songs = new Dictionary<string, string>();

            wait = true;
            useAuth = true;

            current_index = 0;

            Name = "Music";
            ID = "151";

            _nexter = new System.Timers.Timer(3000);
            _nexter.Elapsed += new ElapsedEventHandler(NextSong);

            SongsPlayed = new List<string>();
        }

        public override void Initialize()
        {
            Task[] tasks = new Task[2];

            tasks[0] = Task.Factory.StartNew(() => start_LocalApi());
            tasks[1] = Task.Factory.StartNew(() => start_WebApi());

            Task.WaitAll(tasks);

            ReadSongs();
            GatherPlaylists();

            Brain.Internal.IsPlayingSpotify = musicHandler.IsPlaying();
            Track currTrack = musicHandler.GetCurrentTrack();

            var tid = currTrack.GetTrackURI();
 
            song_current = webAPI.GetTrack(tid);
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
            if (Brain.Internal.isQuiet)
            {
                return "I am in quiet mode sir";
            }

            string input = Brain.Senses.UserInput;

            string[] skippers = new string[] { "next", "skip" };
            string[] players = new string[] { "keep playing", "resume" };
            string[] stoppers = new string[] { "stop", "go quiet", "pause" };
            string[] questions = new string[] { "what song" };

            string response = string.Empty;

            if(input.Equals("update songs"))
            {
                UpdateSongs();
                response = "Songs have been updated sir";
            }
            else if(!String.IsNullOrEmpty(Brain.Senses.Search))
            {
                string name = Brain.Senses.Search;

                Play(name);
                Brain.Internal.IsPlayingSpotify = true;
            }
            else if(players.Any(str => input.Contains(str)))
            {
                Resume();
            }
            else if (skippers.Any(str => input.Contains(str)))
            {
                Next();
            }
            else if (stoppers.Any(str => input.Contains(str)))
            {
                Pause();
            }
            else if (questions.Any(str => input.Contains(str)))
            {
                response = String.Format("This is {0} by {1}", song_current.Name, song_current.Artists.First().Name);
            }
            else
            {
                string[] key = categories.Keys.FirstOrDefault(ks => ks.Any(k => input.Contains(k)));

                if (key != null)
                {
                    Pause();
                    response = "Now playing your" + categories[key] + " playlist";
                    Play(categories[key].ToLower());
                }
                else { response = "I am unsure what to play"; }
            }

            return response;
        }

        void ReadSongs()
        {
            string[] lines = File.ReadAllLines(SONGS_FILE);
            string[] splitter;
            string name, uri;

            foreach (string line in lines)
            {
                splitter = line.Split('@');
                name = splitter[0];
                uri = splitter[1];

                if (!Songs.ContainsKey(name))
                {
                    Songs.Add(name, uri);
                }
            }
        }

        void GatherPlaylists()
        {
            Paging<SimplePlaylist> lists = webAPI.GetUserPlaylists(USER_ID);

            foreach (SimplePlaylist l in lists.Items)
            {
                Playlists.Add(l.Name.ToLower(), l.Id);
            }

            while (lists.Next != null)
            {
                lists = DownloadData<Paging<SimplePlaylist>>(lists.Next, new WebClient());

                foreach (SimplePlaylist l in lists.Items)
                {
                    Playlists.Add(l.Name.ToLower(), l.Id);
                }
            }
        }

        void UpdateSongs()
        {
            string url = "https://api.spotify.com/v1/me/tracks";
            Paging<SavedTrack> tracks = DownloadData<Paging<SavedTrack>>(url, new WebClient());

            int sng = tracks.Total;
            int per = 20;

            int num_tasks = (sng / per) + 1;

            Task[] songers = new Task[num_tasks];

            for (int i = 0; i < num_tasks; i++)
            {
                int start = i * per;
                int end = start + per;

                songers[i] = Task.Factory.StartNew(() => GatherSongs(start, end, new WebClient()));
            }

            Task.WaitAll(songers);

            string[] lines = new string[Songs.Count];
            string key = string.Empty;
            string value = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                key = Songs.Keys.ElementAt(i);
                value = Songs[key];

                lines[i] = key.ToLower() + "@" + value;
            }

            File.WriteAllLines(Brain.FilePaths.Songs, lines);
        }

        void GatherSongs(int start, int end, WebClient client)
        {
            int index = start;

            while (index <= end - 20)
            {
                string url = "https://api.spotify.com/v1/me/tracks?offset=" + index + "&limit=20";
                Paging<SavedTrack> tracks = DownloadData<Paging<SavedTrack>>(url, client); ;

                foreach (SavedTrack t in tracks.Items)
                {
                    if (!Songs.ContainsKey(t.Track.Name))
                    {
                        Songs.Add(t.Track.Name, t.Track.Uri);
                    }
                }

                index += 20;
            }
        }

        public void Pause()
        {
            musicHandler.Pause();
            Brain.Internal.IsPlayingSpotify = false;
        }

        public void Resume()
        {
            musicHandler.Play();
            Brain.Internal.IsPlayingSpotify = true;
        }

        public void Play(string str)
        {
            str = str.ToLower();
            string uri = string.Empty, key = string.Empty;

            if(Playlists.ContainsKey(str))
            {
                uri = Playlists[str];
                playlist_current = webAPI.GetPlaylist(USER_ID, Playlists[str]);

                PlayPlaylist(Playlists[str]);
            }
            else if (Songs.Keys.Any(k => k.ToLower().Contains(str)))
            {
                uri = GetUriFromName(str);
            
                song_current = webAPI.GetTrack(uri);
                current_index = Songs.Keys.ToList().FindIndex(sng => sng.Equals(str));
                playlist_current = null;

                PlayTrack(song_current);
            }
        }

        private string GetUriFromName(string name)
        {
            string key = Songs.Keys.FirstOrDefault(k => k.ToLower().Equals(name.ToLower()));
            string uri = string.Empty;

            if (key != null)
            {
                uri = Songs[key];
            }
            else
            {
                uri = Songs[Songs.Keys.FirstOrDefault(k => k.Contains(name))];
            }

            return uri;
        }

        public void PlayPlaylist(string uri)
        {
            Paging<PlaylistTrack> play = webAPI.GetPlaylistTracks("mschuee", uri);
            FullTrack track = new FullTrack();
            Random rand = new Random();
            int index, max;

            max = play.Limit > play.Items.Count ? play.Items.Count : play.Limit;
            index = rand.Next(max);

            track = play.Items.ElementAt(index).Track;

            current_index = index;
            song_current = track;

            PlayTrack(track);
        }

        public void Next()
        {
            if(playlist_current != null)
            {
                Paging<PlaylistTrack> tracksInPlaylist = webAPI.GetPlaylistTracks("mschuee", playlist_current.Id);
                Random rand = new Random();

                List<PlaylistTrack> tracksNotPlayed = tracksInPlaylist.Items
                    .Where(track => !SongsPlayed.Contains(track.Track.Id))
                    .ToList();

                if(tracksNotPlayed.Count == 0)
                {
                    SongsPlayed.Clear();
                    tracksNotPlayed = tracksInPlaylist.Items;
                }

                int index = rand.Next(tracksNotPlayed.Count);

                current_index = index;
                FullTrack trackToBePlayed = tracksNotPlayed.ElementAt(index).Track;
                song_current = trackToBePlayed;

                PlayTrack(trackToBePlayed);
            }
            else
            {
                Random rand = new Random();
                int new_index = 0;

                while (new_index == current_index)
                {
                    new_index = rand.Next(Songs.Count);
                }

                PlayTrackByName(Songs.Keys.ElementAt(new_index));
                current_index = new_index;
            }
        }

        public void PlayTrack(FullTrack track)
        {
            SongsPlayed.Add(track.Id);

            musicHandler.PlayURL(track.Uri);

            int wait = track.DurationMs;

            _nexter.Stop();

            _nexter = new System.Timers.Timer(wait);
            _nexter.Elapsed += new ElapsedEventHandler(NextSong);

            _nexter.Start();

            Brain.Internal.IsPlayingSpotify = true;
        }

        public void NextSong(object sender, ElapsedEventArgs e)
        {
            Next();
        }

        public void PlayTrackByName(string name)
        {
            name = name.ToLower();

            if(Songs.ContainsKey(name))
            {
                musicHandler.PlayURL(Songs[name]);
                Brain.Internal.IsPlayingSpotify = true;
            }
        }

        FullPlaylist GetPlaylist(string name)
        {
            FullPlaylist list = webAPI.GetPlaylist(USER_ID, Playlists[name]);
            playlist_current = list;

            return list;
        }

        FullTrack GetTrack(FullPlaylist list, string name)
        {
            FullTrack track = list.Tracks.Items.FirstOrDefault(tr => tr.Track.Name.Equals(name)).Track;
            song_current = track;

            return track;
        }

        void start_WebApi()
        {
            authorizer = new AutorizationCodeAuth()
            {
                ClientId = CLIENT_ID,
                RedirectUri = "http://localhost",
                Scope = Scope.USER_READ_PRIVATE | Scope.USER_READ_EMAIL | Scope.PLAYLIST_READ_PRIVATE | Scope.USER_LIBRARAY_READ | Scope.USER_LIBRARY_MODIFY | Scope.USER_READ_PRIVATE
                    | Scope.USER_FOLLOW_MODIFY | Scope.USER_FOLLOW_READ | Scope.PLAYLIST_MODIFY_PRIVATE | Scope.USER_READ_BIRTHDATE
            };

            string[] lines = File.ReadAllLines(KEYS_FILE);

            if(lines.Length == 2)
            {
                ACCESS_TOKEN = lines[0];
                REFRESH_TOKEN = lines[1];
            }
          
            webAPI = new SpotifyWebAPIClass()
            {
                AccessToken = ACCESS_TOKEN,
                TokenType = "Bearer",
                UseAuth = true
            };

            var test = webAPI.GetPrivateProfile();
     
            if (test.Id == null)
            {
                Token refresh = authorizer.RefreshToken(REFRESH_TOKEN, SECRET);

                ACCESS_TOKEN = refresh.AccessToken;

                string[] lns = new string[] { refresh.AccessToken, REFRESH_TOKEN };
                File.WriteAllLines(KEYS_FILE, lns);

                webAPI = new SpotifyWebAPIClass()
                {
                    AccessToken = ACCESS_TOKEN,
                    TokenType = "Bearer",
                    UseAuth = true
                };
            }

            test = webAPI.GetPrivateProfile();

            if(test.Id == null)
            {
                authorizer.StartHttpServer();
                authorizer.OnResponseReceivedEvent += OnResponse;
                authorizer.DoAuth();

                while (wait) { }
            }
        }

        void OnResponse(AutorizationCodeAuthResponse resp)
        {
            Token t = authorizer.ExchangeAuthCode(resp.Code, SECRET);

            ACCESS_TOKEN = t.AccessToken;
            REFRESH_TOKEN = t.RefreshToken;

            webAPI = new SpotifyWebAPIClass()
            {
                AccessToken = ACCESS_TOKEN,
                TokenType = "Bearer",
                UseAuth = true
            };

            string[] lines = new string[] { ACCESS_TOKEN, REFRESH_TOKEN };
            File.WriteAllLines(KEYS_FILE, lines);

            wait = false;
        }

        void start_LocalApi() 
        {
            localAPI = new SpotifyLocalAPIClass();

            if (!SpotifyLocalAPIClass.IsSpotifyRunning())
            {
                localAPI.RunSpotify();
                Thread.Sleep(5000);
            }

            if (!SpotifyLocalAPIClass.IsSpotifyWebHelperRunning())
            {
                localAPI.RunSpotifyWebHelper();
                Thread.Sleep(4000);
            }

            if (!localAPI.Connect())
            {
                MessageBox.Show("Couldn't connect");
            }

            musicHandler = localAPI.GetMusicHandler();

            localAPI.Update();
        }

        private T DownloadData<T>(String url, WebClient web)
        {
            string xxx = DownloadString(url, web);
            return JsonConvert.DeserializeObject<T>(xxx, settings);
        }

        private String DownloadString(String url, WebClient webclient)
        {
            if (useAuth)
                webclient.Headers.Set("Authorization", "Bearer" + " " + ACCESS_TOKEN);
            else if (!useAuth && webclient.Headers["Authorization"] != null)
                webclient.Headers.Remove("Authorization");
            String response = "";
            try
            {
                byte[] data = webclient.DownloadData(url);
                response = Encoding.UTF8.GetString(data);
                int sto = 0;
            }
            catch (Exception e)
            {

            }
            return response;
        }
    }
}
