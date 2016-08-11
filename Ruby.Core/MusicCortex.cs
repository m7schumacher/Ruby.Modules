using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;

namespace Atlas.Internal
{
    public class MusicCortex
    {
        public string SongCurrent { get; set; }
        public string URI_SongCurrent { get; set; }

        public string PlaylistCurrent { get; set; }
        public string URI_PLaylistCurrent { get; set; }

        public MusicCortex()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            SongCurrent = string.Empty;
            URI_SongCurrent = string.Empty;

            PlaylistCurrent = string.Empty;
            URI_PLaylistCurrent = string.Empty;
        }

        public void UpdateCurrentSong(Track track)
        {
            SongCurrent = track.TrackResource.Name;
            URI_SongCurrent = track.TrackResource.Uri;
        }
    }
}
