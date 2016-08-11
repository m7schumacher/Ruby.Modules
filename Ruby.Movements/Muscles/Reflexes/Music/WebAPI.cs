using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Swiss;
using Newtonsoft.Json;
using Ruby.Internal;

namespace Ruby.Muscle.Music
{
    public class WebAPI
    {
        private SpotifyWebAPI _spotify;
        private ImplicitGrantAuth _auth;

        JsonSerializerSettings settings;

        private PrivateProfile _profile;
        private List<FullTrack> _savedTracks;
        private List<SimplePlaylist> _playlists;

        public List<SimplePlaylist> Playlists
        {
            get { return _playlists; }
        }

        static AutorizationCodeAuth authorizer;

        string ACCESS_TOKEN = string.Empty;
        string REFRESH_TOKEN = string.Empty;

        bool wait = true;
        bool useAuth;

        public Dictionary<string, string> Songs { get; set; }

        public WebAPI(string clientID, string keys)
        {
            Songs = new Dictionary<string, string>();

            useAuth = true;

            settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };

            _savedTracks = new List<FullTrack>();
            authorizer = new AutorizationCodeAuth
            {
                RedirectUri = "http://localhost",
                ClientId = clientID,
                Scope = Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead | Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate,
                State = "XSS"
            };

            string[] lines = File.ReadAllLines(keys);

            if (lines.Length == 2)
            {
                ACCESS_TOKEN = lines[0];
                REFRESH_TOKEN = lines[1];
            }

            _spotify = new SpotifyWebAPI()
            {
                AccessToken = ACCESS_TOKEN,
                TokenType = "Bearer",
                UseAuth = true
            };

            var test = _spotify.GetPrivateProfile();

            if (test.Id == null)
            {
                Token refresh = authorizer.RefreshToken(REFRESH_TOKEN, Core.FilePaths.SpofityClientSecret);

                ACCESS_TOKEN = refresh.AccessToken;

                string[] lns = new string[] { refresh.AccessToken, REFRESH_TOKEN };
                File.WriteAllLines(keys, lns);

                _spotify = new SpotifyWebAPI()
                {
                    AccessToken = ACCESS_TOKEN,
                    TokenType = "Bearer",
                    UseAuth = true
                };
            }

            test = _spotify.GetPrivateProfile();

            if (test.Id == null)
            {
                authorizer.StartHttpServer();
                authorizer.OnResponseReceivedEvent += OnResponse;
                authorizer.DoAuth();
            }

            InitialSetup();

            while (wait) { }

        }

        //private void _auth_OnResponseReceivedEvent(Token token, string state)
        //{
        //    _auth.StopHttpServer();

        //    if (state != "XSS")
        //    {
        //        MessageBox.Show(@"Wrong state received.", @"SpotifyWeb API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //    if (token.Error != null)
        //    {
        //        MessageBox.Show("Error: {token.Error}", @"SpotifyWeb API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    _spotify = new SpotifyWebAPI
        //    {
        //        UseAuth = true,
        //        AccessToken = token.AccessToken,
        //        TokenType = token.TokenType
        //    };
        //    InitialSetup();
        //}

        void OnResponse(AutorizationCodeAuthResponse resp)
        {
            Token t = authorizer.ExchangeAuthCode(resp.Code, Core.FilePaths.SpofityClientSecret);

            ACCESS_TOKEN = t.AccessToken;
            REFRESH_TOKEN = t.RefreshToken;

            _spotify = new SpotifyWebAPI()
            {
                AccessToken = ACCESS_TOKEN,
                TokenType = "Bearer",
                UseAuth = true
            };

            string[] lines = new string[] { ACCESS_TOKEN, REFRESH_TOKEN };
            File.WriteAllLines(Core.FilePaths.Song_Keys, lines);

            wait = false;
        }

        private void _auth_OnResponseReceivedEvent(Token token, string state)
        {
            _auth.StopHttpServer();

            if (state != "XSS")
            {
                MessageBox.Show(@"Wrong state received.", @"SpotifyWeb API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (token.Error != null)
            {
                MessageBox.Show("Error: {token.Error}", @"SpotifyWeb API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _spotify = new SpotifyWebAPI
            {
                UseAuth = true,
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
        }

        private async void InitialSetup()
        {
            _profile = _spotify.GetPrivateProfile();
            ReadSongs();
            _playlists = GetPlaylists();

            Songs = Songs.SortByKey(str => str);

            wait = false;
        }

        void ReadSongs()
        {
            string[] lines = File.ReadAllLines(Core.FilePaths.Songs);
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

        public void UpdateSongs()
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

            File.WriteAllLines(Core.FilePaths.Songs, lines);
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

        private List<FullTrack> GetSavedTracks()
        {
            Paging<SavedTrack> savedTracks = _spotify.GetSavedTracks();
            List<FullTrack> list = new List<FullTrack>();

            if (savedTracks.Items != null)
            {
                list = savedTracks.Items.Select(track => track.Track).ToList();

                while (savedTracks.Next != null)
                {
                    savedTracks = _spotify.GetSavedTracks(20, savedTracks.Offset + savedTracks.Limit);
                    list.AddRange(savedTracks.Items.Select(track => track.Track));
                }
            }

            return list;
        }

        public string GetTrackURIFromName(string name)
        {
            var target = Songs.Keys.FirstOrDefault(track => track.EqualsIgnoreCase(name));

            if(target == null)
                target = Songs.Keys.FirstOrDefault(track => track.ContainsIgnoreCase(name));

            if(target != null)
                return Songs[target];

            return string.Empty;
        }

        public string GetPlaylistURIFromName(string name)
        {
            var target = _playlists.FirstOrDefault(play => play.Name.ContainsIgnoreCase(name));

            if (target != null)
            {
                return target.Uri;
            }

            return string.Empty;
        }

        private List<SimplePlaylist> GetPlaylists()
        {
            Paging<SimplePlaylist> playlists = _spotify.GetUserPlaylists(_profile.Id);
            List<SimplePlaylist> list = playlists.Items.ToList();

            while (playlists.Next != null)
            {
                playlists = _spotify.GetUserPlaylists(_profile.Id, 20, playlists.Offset + playlists.Limit);
                list.AddRange(playlists.Items);
            }

            return list;
        }

        private void authButton_Click(object sender, EventArgs e)
        {
            _auth.StartHttpServer(8000);
            _auth.DoAuth();
        }
    }
}
