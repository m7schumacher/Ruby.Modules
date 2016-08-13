using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using System.Windows.Forms;
using Ruby.Internal;

namespace Ruby.Movements.Music
{
    public class LocalAPI
    {
        private readonly SpotifyLocalAPI _spotify;
        private Track _currentTrack;

        public LocalAPI()
        {
            _spotify = new SpotifyLocalAPI();
            _spotify.OnPlayStateChange += _spotify_OnPlayStateChange;
            _spotify.OnTrackChange += _spotify_OnTrackChange;
            _spotify.OnTrackTimeChange += _spotify_OnTrackTimeChange;
            _spotify.OnVolumeChange += _spotify_OnVolumeChange;

            Connect();
        }

        private void _spotify_OnVolumeChange(object sender, VolumeChangeEventArgs e) { }
        private void _spotify_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e) { }

        private void _spotify_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            Core.Internal.IsPlayingSpotify = e.Playing;
        }

        private void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            Core.Music.UpdateCurrentSong(e.NewTrack);
            UpdateTrack(e.NewTrack);
        }

        public void Connect()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                SpotifyLocalAPI.RunSpotify();
                while (!SpotifyLocalAPI.IsSpotifyRunning()) { }
            }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                MessageBox.Show(@"SpotifyWebHelper isn't running!");
                return;
            }

            bool successful = _spotify.Connect();
            if (successful)
            {
                UpdateInfos();
                _spotify.ListenForEvents = true;
            }
            else
            {
                DialogResult res = MessageBox.Show(@"Couldn't connect to the spotify client. Retry?", @"Spotify", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                    Connect();
            }
        }

        public void UpdateInfos()
        {
            StatusResponse status = _spotify.GetStatus();
            if (status == null)
                return;

            Core.Internal.IsPlayingSpotify = status.Playing;

            if (status.Track != null) //Update track infos
                UpdateTrack(status.Track);
        }

        public async void UpdateTrack(Track track)
        {
            _currentTrack = track;
        }

        public bool IsPlaying()
        {
            return _spotify.GetStatus().Playing;
        }

        public void Play()
        {
            _spotify.Play();
        }

        public void Play(string uri)
        {
            _spotify.PlayURL(uri, string.Empty);
        }

        public void Pause()
        {
            _spotify.Pause();
        }

        public void Previous()
        {
            _spotify.Previous();
        }

        public void Next()
        {
            _spotify.Skip();
        }

        private static String FormatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }

        public string CurrentSongString()
        {
            return String.Format("This is {0} by {1}", _currentTrack.TrackResource.Name, _currentTrack.ArtistResource.Name);
        }

        public string CurrentSongName()
        {
            return _currentTrack.TrackResource.Name;
        }

        public string CurrentSongArtist()
        {
            return _currentTrack.ArtistResource.Name;
        }
    }
}
