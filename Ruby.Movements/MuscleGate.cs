using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Swiss;

namespace Ruby.Movements
{
    public class MuscleGate
    {
        private static MuscleGate _gate;
        internal static SetOfSkills Skills { get; set; }

        public static MuscleGate Gate 
        { 
            get 
            {
                _gate = _gate ?? new MuscleGate();
                return _gate;
            } 
        }

        public MuscleGate()
        {
            Skills = new SetOfSkills();
        }

        public string React(string id)
        {
            string response = string.Empty;
            Response responder = Skills.GetResponseByID(id);

            if(responder != null)
            {
                response = responder.Execute();
            }
            else
            {
                response = GetWolframAnswer();
                response = String.IsNullOrEmpty(response) ? "I am unsure what to do" : response;
            }

            return response;
        }

        public Dictionary<string, Dictionary<string, string[]>> GetRecognizers()
        {
            Dictionary<string, Dictionary<string, string[]>> recogs = new Dictionary<string, Dictionary<string, string[]>>();

            foreach(Response resp in Skills.Responses.Where(rep => rep.RecognizedPhrases != null))
            {
                recogs.Add(resp.ID, resp.RecognizedPhrases);
            }

            return recogs;
        }

        public static void Dream()
        {
            Spotify wt = new Spotify();
            wt.Initialize();

            wt.Execute();
        }

        public void PlaySong(string song)
        {
            Skills.Spotify.Play(song);
        }

        public void ResumeSpotify()
        {
            Skills.Spotify.Local.Play();
        }

        public void PauseSpotify()
        {
            Skills.Spotify.Local.Pause();
        }

        public List<string> GetSongsFromSpotify()
        {
            return Skills.Spotify.Web.Songs.Keys.ToList();
        }

        public string GetWolframAnswer()
        {
            return Skills.Wolfram.GetAnswer();
        }

        public void SendTextMessage(string subject, string message)
        {
            //Skills.Phone.SendTextMessage(subject, message, 7018935034);
        }

        public static void Terminate()
        {
            Terminator term = new Terminator();
            term.Execute();
        }

        //public string ReactToExternal(string id)
        //{
        //    string response = string.Empty;
        //    Response responder = Skills.GetResponseByID(id);

        //    if(responder != null && responder.External)
        //    {
        //        response = responder.Execute();
        //    }
        //    else if(!responder.External)
        //    {
        //        response = "I'm afraid I can only do that locally sir";
        //    }
        //    else if(responder == null)
        //    {
        //        response = GetWolframAnswer();
        //        response = String.IsNullOrEmpty(response) ? "I am unsure what to do" : response;
        //    }

        //    return response;
        //}
    }
}
