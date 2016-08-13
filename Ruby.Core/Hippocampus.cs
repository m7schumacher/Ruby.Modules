using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Swiss;

namespace Ruby.Internal
{
    public class Hippocampus
    {
        public bool IsPlayingSpotify { get; set; }
        public bool IsConnectedToInternet { get; set; }
        public bool IsConnectedToSpotify { get; set; }
        
        public Hippocampus()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            IsPlayingSpotify = false;
            IsConnectedToInternet = false;
            IsConnectedToSpotify = false;

            Task tsk = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(60000);
                UpdateValues();
            }); 
        }

        private void UpdateValues()
        {
            //IsConnectedToInternet = InternetUtility.CheckForInternetConnection();
        }
    }
}
