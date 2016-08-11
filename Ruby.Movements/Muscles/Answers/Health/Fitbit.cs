using System;
using System.Collections.Generic;
using Fitbit.Api;
using System.IO;
using Fitbit.Models;
using System.Diagnostics;
using System.Xml.Serialization;
using Ruby.Internal;

namespace Ruby.Muscle
{
    internal class Fitbit : Answer
    {
        static string ConsumerKey;
        static string ConsumerSecret;

        static DateTime today = DateTime.Now;

        UserProfile profile;
        FitbitClient client;

        public Fitbit() : base()
        {
            ConsumerKey = Core.FilePaths.FitbitConsumerKey;
            ConsumerSecret = Core.FilePaths.FitbitConsumerSecret;
           
            Name = "Fitbit";
            Key = "fitbit";
            ID = "79";
        }

        public override void Initialize()
        {
            var credentials = LoadCredentials();

            if (credentials == null)
            {
                credentials = Authenticate();
                SaveCredentials(credentials);
            }

            client = new FitbitClient(ConsumerKey, ConsumerSecret, credentials.AuthToken, credentials.AuthTokenSecret);
            
            try
            {
                profile = client.GetUserProfile();
            }
            catch(Exception e)
            {
                Disable();
                return;
            }
        }

        public override void EstablishRecognizers()
        {
            Recognizers = new Dictionary<string, string[]>()
            {
                { "tell me my {} today", new string[] { "footsteps", "calories", "distance traveled", "sleep", "activity" } },
                { "how many {} have I || today", new string[] { "steps", "footsteps", "calories", "miles", "hours" } },
            };
        }

        public override void GatherValues()
        {
            Values = new Dictionary<string, string>()
            {
                { "steps", GetSteps().ToString() },
                { "calories", GetCalories().ToString() },
                { "heart", GetHeartRate().ToString() },
                { "distance", GetDistanceTraveled().ToString() },
                { "sleep", GetSleep() },
                { "activity", GetTimeInZone().ToString() },
            };
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>()
            {
                { "steps", new string[] { "steps", "how many steps" } },
                { "calories", new string[] { "calories", "how many calories" } },
                { "heart", new string[] { "resting heart rate", "heart rate", "heart" } },
                { "distance", new string[] { "how far", "how far have I walked", "distance walked", "distance" } },
                { "sleep", new string[] { "how much sleep", "sleep" } },
                { "activity", new string[] { "active", "how active have" } },
            };
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = string.Empty;
        }

        public int GetGoal_Calories()
        {
            return client.GetDayActivity(today).Goals.CaloriesOut;
        }

        public int GetGoal_Steps()
        {
            return client.GetDayActivity(today).Goals.Steps;
        }

        public int GetCalories()
        {
            return client.GetDayActivitySummary(today).CaloriesOut;
        }

        public int GetSteps()
        {
            return client.GetDayActivitySummary(today).Steps;
        }

        public string GetSleep()
        {
            int minutesAsleep = client.GetSleep(today).Summary.TotalMinutesAsleep;

            int hours = minutesAsleep / 60;
            int minutes = minutesAsleep % 60;

            return hours + " hours and " + minutes + " minutes";
        }

        public int GetHeartRate()
        {
            return client.GetHeartRates(today).Average[0].HeartRate;
        }

        public int GetDistanceTraveled()
        {
            return (int)client.GetDayActivitySummary(today).Distances[0].Distance;
        }

        public int GetTimeInZone()
        {
            return (int)client.GetDayActivitySummary(today).VeryActiveMinutes;
        }

        static AuthCredential Authenticate()
        {
            var requestTokenUrl = "http://api.fitbit.com/oauth/request_token";
            var accessTokenUrl = "http://api.fitbit.com/oauth/access_token";
            var authorizeUrl = "http://www.fitbit.com/oauth/authorize";

            var a = new Authenticator(ConsumerKey, ConsumerSecret, requestTokenUrl, accessTokenUrl, authorizeUrl);

            RequestToken token = a.GetRequestToken();

            var url = a.GenerateAuthUrlFromRequestToken(token, false);

            Process.Start(url);

            string pin = Microsoft.VisualBasic.Interaction.InputBox("Prompt", "Title", "Default", -1, -1);

            var credentials = a.GetAuthCredentialFromPin(pin, token);
            return credentials;
        }

        static void SaveCredentials(AuthCredential credentials)
        {
            try
            {
                var path = GetAppDataPath();
                var serializer = new XmlSerializer(typeof(AuthCredential));
                TextWriter writer = new StreamWriter(path);
                serializer.Serialize(writer, credentials);
                writer.Close();
            }
            catch (Exception ex)
            {
                
            }
        }
        static AuthCredential LoadCredentials()
        {
            AuthCredential credentials = null;
            try
            {
                var path = GetAppDataPath();

                if (File.Exists(path))
                {
                    var serializer = new XmlSerializer(typeof(AuthCredential));
                    FileStream fs = new FileStream(path, FileMode.Open);

                    credentials = serializer.Deserialize(fs) as AuthCredential;
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                
            }

            return credentials;
        }

        static string GetAppDataPath()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fitbit");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, "Credentials.xml");
        }
    }
}
