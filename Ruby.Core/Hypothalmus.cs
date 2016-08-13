using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Swiss;

namespace Ruby.Internal
{
    public class Hypothalmus
    {
        public bool isLocal { get; set; }
        public bool isSpotifyOpen { get; set; }
        public bool IsPlayingSpotify { get; set; }

        public bool isAwake { get; set; }
        public bool isUsingKinect { get; set; }
        public bool isInDashMode { get; set; }

        public bool isQuiet { get; set; }
        public bool isDreaming { get; set; }
        public bool isLearning { get; set; }

        public bool IsConnectedToInternet { get; set; }
        public bool IsConnectedToSpotify { get; set; }

        public enum States { Cold, Warm, Ready };
        public States State { get; set; }

        string[] linesOfSettingsFile;

        public void InitializeValues()
        {
            linesOfSettingsFile = File.ReadAllLines(Core.FilePaths.SettingsFile);

            isQuiet = GetSetting("quiet");
            isDreaming = GetSetting("dreaming");
            isLearning = GetSetting("learning");

            isAwake = !isDreaming;
            isInDashMode = false;
            isSpotifyOpen = false;
            isUsingKinect = false;
            IsPlayingSpotify = false;
            IsConnectedToInternet = false;
            IsConnectedToSpotify = false;
            isLocal = true;

            State = States.Cold;

            Task tsk = Task.Factory.StartNew(() =>
            {
                UpdateValues();
                Thread.Sleep(60000);
            });
        }

        private bool GetSetting(string setting)
        {
            var value = linesOfSettingsFile.FirstOrDefault(line => line.Contains(setting));

            if (value != null)
            {
                return value.Split('=')[1].ToLower().Equals("true");
            }

            return false;
        }

        private void UpdateValues()
        {
            IsConnectedToInternet = InternetUtility.CheckForInternetConnection();
        }
    }
}
