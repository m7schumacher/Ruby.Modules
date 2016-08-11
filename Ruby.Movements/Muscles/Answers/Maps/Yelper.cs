using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YelpSharp.Data;
using YelpSharp.Util;
using Newtonsoft.Json;
using System.Xml.Linq;
using Google.Maps.Direction;
using YelpSharp;
using System.Configuration;
using YelpSharp.Data.Options;
using System.Diagnostics;
using Ruby.Internal;

namespace Ruby.Muscle
{
    internal class Yelper : Answer
    {
        double radius;
        string key, core, type, search;

        string Consumer_key;
        string Consumer_secret;
        string Token_key;
        string Token_secret;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public Dictionary<string, string> parameters;

        Options options;
        Yelp yelper;

        public Yelper() : base()
        {
            options = new Options()
            {
                AccessToken = "_aKiFyzUkZMPOd057o0dYR8hCUcAPyhU",
                AccessTokenSecret = "zbnu1quW187TV5V4xvUn-XKF8M0",
                ConsumerKey = "kbc11jamH_s7carpDGocog",
                ConsumerSecret = "ll_Dm277MjjJg4t9ciGlCjmXcPI"
            };

            Name = "Yelper";
            Key = "yelp";
            ID = "154";

            Spec = "gen";
        }

        public override void Initialize()
        {
            yelper = new Yelp(options);   
        }

        public override void EstablishRecognizers()
        {
            Recognizers = new Dictionary<string, string[]>()
            {
                { "{} is the nearest ||", new string[] { "where", "how far" } },
                { "I'm hungry for some {}", new string[] { "fast food", "good food" } },
                { "I need to ||", new string[] { } },
            };
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>()
            {
                { "distance", new string[]{ "how far", "where is", "nearest" } },
                { "food", new string[] { "hungry", "food", "restaurant", "fast food", "sloppy", "cheap restaurant" } },
                { "gas", new string[] { "low on gas", "fill up", "gas", "car", "gas station" } },
                { "store", new string[] { "store", "goods", "shopping" } },
                { "retail", new string[] { "retailer", "big store", "discount" } },
            };
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>()
            {
                { "_address", new string[] { "where", "address of", "location of", "location" } },
                { "_number", new string[] { "number", "phone number", "phone" } },
                { "_web", new string[] { "website", "url", "site" } },
                { "_name", new string[] { "what is the name", "the name", "name", "called" } }
            };     
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = string.Empty;
        }

        public override void WarmUp()
        {
            search = Core.External.Search;
        }

        public override void GatherValues()
        {
            search = !String.IsNullOrEmpty(search) ? search : Spec.Split('_')[0];

            Business result = MakeCall(search);

            Values = new Dictionary<string, string>()
            {
                { "dist", ConvertMetersToMiles(result.distance).ToString() },
                { "name", result.name },
                { "number", result.phone },
                { "url", result.url },
                { "address", result.location.address[0] }
            };
        }

        private void GoToSite()
        {
            Process firstProc = new Process();
            string add = Values["address"];

            if(add.Length > 0)
            {
                firstProc.StartInfo.FileName = Core.FilePaths.GoogleURL + add;
                firstProc.Start();
            }
        }

        private static double ConvertMetersToMiles(double meters)
        {
            return System.Math.Round(meters / 1609.344, 1);
        }

        public Business MakeCall(string keyword)
        {
            List<Business> SearchList = new List<Business>();
            List<Business> l = new List<Business>();
            SearchOptions searchOptions = new SearchOptions();

            searchOptions.GeneralOptions = new GeneralOptions()
            {
                term = keyword
            };

            searchOptions.LocationOptions = new LocationOptions()
            {
                location = Core.Location.Address
            };

            var task = yelper.Search(searchOptions).Result.businesses;

            var nearest = task.First();

            return nearest;
        }

        
        private bool LookingFor(XElement x, string search)
        {
            string target = x.Element("name").Value;
            return target != null ? target.Equals(search) : false;
        }

        private void RequestCompleted(IAsyncResult result)
        {
            string xml = string.Empty;

            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);

            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                xml = r.ReadToEnd();
            }

            XElement sitemap = XElement.Parse(xml);
            List<XElement> results = sitemap.Elements().ToList();
            results.RemoveAt(0);

            XElement elem = results.FirstOrDefault(x => LookingFor(x, "ProSpect Park"));

            string name = elem.Element("name").Value;
            string lat = elem.Element("geometry").Element("location").Element("lat").Value;
            string lon = elem.Element("geometry").Element("location").Element("lng").Value;
            string address = elem.Element("vicinity").Value;

            string place_id = elem.Element("place_id").Value;
      
            var dir = new DirectionRequest();
            dir.Origin = Core.Location.Address;
            dir.Destination = new Google.Maps.Location(lat + "," + lon);
            dir.Sensor = false;

            var resp = new DirectionService().GetResponse(dir);
        }
    }
}
