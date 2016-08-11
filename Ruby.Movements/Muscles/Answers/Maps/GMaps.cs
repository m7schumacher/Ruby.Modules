using System;
using System.Collections.Generic;
using System.Linq;
using Google.Maps.Geocoding;
using Google.Maps.Direction;
using Swiss;
using Ruby.Internal;

namespace Ruby.Muscle
{
    internal class GMaps : Answer
    {
        string destination;

        public GMaps() : base()
        {
            Name = "Map";
            Key = "map";
            ID = "152";

            Spec = "where";
        }

        public override void EstablishRecognizers()
        {
            Recognizers = new Dictionary<string, string[]>()
            {
                { "what is my current {}", new string[] { "location", "address" } },
                { "where is {}", new string[] { "home", "work", "the apartment" } },
                { "what {} am I in", new string[] { "city", "town", "country", "zipcode" } },
                { "how long || to get {}", new string[] { "to the apartment", "home", "to work" } },
                { "how far is {}", new string[] { "work", "home", "the apartment" } }
            };
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>()
            {
                { "gen", new string[] { "directions", "route" } },
                { "where", new string[] { "where am I", "where am i", "location", "address" } },
                { "target", new string[] { "where is" } },
                { "time", new string[] { "how long", "time", "take", "minutes", "hours" } },
                { "dist", new string[] { "how far", "miles", "distance" } },
                { "city", new string[] { "city", "town" } },
                { "state", new string[] { "state" } },
                { "country", new string[] { "country" } },
                { "zipcode", new string[] { "zipcode" } },
            };
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = "gen";
        }

        public override void GatherValues()
        {
            string input = Core.External.UserInput.ToLower();

            while (String.IsNullOrEmpty(Core.Location.Address)) { }

            Values["current"] = Core.Location.Address;
            Values["zipcode"] = Core.Location.ZipCode;
            Values["city"] = Core.Location.City;
            Values["state"] = Core.Location.State;
            Values["country"] = Core.Location.Country;

            if (Spec.Equals("target") || Spec.Equals("where"))
            {
                string match = Core.Location.StoredAddresses.Keys.FirstOrDefault(k => input.ToLower().Contains(k.ToLower()));

                if(match != null)
                {
                    Values["target"] = Core.Location.StoredAddresses[match];
                }
            }
            else if(Spec.Equals("time") || Spec.Equals("dist"))
            {
                Dictionary<string, string> results = new Dictionary<string, string>();
                string[] bits = input.SplitOnWhiteSpace();

                int index = bits.ToList().IndexOf("from");

                if(index > 0)
                {
                    string start = bits[index - 1].Replace("\"", "");
                    start = start.Equals("here") ? Core.Location.Address : start;
                    string end = bits[index + 1].Replace("\"", "");
                    end = end.Equals("here") ? Core.Location.Address : end;

                    bool startIsKnown = Core.Location.StoredAddresses.ContainsKey(start);
                    bool endIsKnown = Core.Location.StoredAddresses.ContainsKey(end);

                    start = startIsKnown ? Core.Location.StoredAddresses[start] : start;
                    end = endIsKnown ? Core.Location.StoredAddresses[end] : end;

                    results = GetDistanceTo(start, end);
                }
                else
                {
                    string match = Core.Location.StoredAddresses.Keys.FirstOrDefault(k => input.ToLower().Contains(k.ToLower()));

                    destination = match != null ? Core.Location.StoredAddresses[match] : Core.External.Search;
                    results = GetDistanceTo(destination);
                }

                var hours = Convert.ToInt16(results["hours"]);

                Values["dist"] = results["distance"];
                Values["hours"] = results["hours"];
                Values["minutes"] = results["minutes"];
                Values["summary"] = results["summary"];

                if(Spec.Equals("time") && hours == 0)
                {
                    Spec += "_min";
                }
            }
        }

        private double ConvertDuration(string dur)
        {
            string[] splitter = dur.Split(' ');
            double hours = 0, minutes = 0;

            if(splitter.Length > 2)
            {
                hours = Convert.ToDouble(splitter[0]);
                minutes = Convert.ToDouble(splitter[2]);
            }
            else
            {
                minutes = Convert.ToDouble(splitter[0]);
            }

            return (hours * 60) + minutes;
        }

        private double ConvertDistance(string dist)
        {
            return Convert.ToDouble(dist.Split(' ')[0]);
        }

        public Dictionary<string, string> GetDistanceTo(string start, string dest)
        {
            var dir = new DirectionRequest();
            dir.Origin = start;
            dir.Destination = dest;
            dir.Sensor = false;

            var resp = new DirectionService().GetResponse(dir);

            double duration = resp.Routes.First().Legs.Sum(l => ConvertDuration(l.Duration.Text));
            double distance = resp.Routes.First().Legs.Sum(l => ConvertDistance(l.Distance.Text));

            string hours = Math.Floor(duration / 60).ToString();
            string minutes = (duration % 60).ToString();
            string summary = resp.Routes.First().Summary;

            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "hours", hours },
                { "minutes", minutes },
                { "distance", distance.ToString() },
                { "summary", summary }
            };

            return values;
        }

        public Dictionary<string,string> GetDistanceTo(string dest)
        {
            return GetDistanceTo(Core.Location.Address, dest);
        }

        public void ShowOnMap(string address)
        {
            //var map = new StaticMapRequest();
            //map.Center = address;
            //map.Size = new System.Drawing.Size(1000, 1000);
            //map.Zoom = 18;
            //map.Sensor = false;

            //Process proc = new Process();

            //proc.StartInfo.FileName = map.ToUri().ToString();
            //proc.Start();
        }

        public string GetAddressInformation(string address)
        {
            var request = new GeocodingRequest();
            request.Address = address;
            
            request.Sensor = false;
            GeocodeResponse response = new GeocodingService().GetResponse(request);

            return response.ToString();
        }
    }
}
