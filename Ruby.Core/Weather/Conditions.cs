using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Internal
{
    public class Conditions
    {
        private Dictionary<string, string> DirectionMappings = new Dictionary<string, string>()
        {
            { " S", " from the south " },
            { " N", " from the north " },
            { " W", " from the west " },
            { " E", " from the east " },
        };

        public string City { get; set; }
        public string DayOfWeek { get; set; }

        public string Condition { get; set; }
        public string Temperature { get; set; }
        public string FeelsLike { get; set; }
        public string Humidity { get; set; }
        public string DewPoint { get; set; }
        public string Visibility { get; set; }
        public string Wind { get; set; }
        public string WindDesc { get; set; }
        public string WindDirection { get; set; }
        public string WindChill { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Precipitation { get; set; }
        public string MoonIllumintion { get; set; }

        public string DirectionNearestStorm { get; set; }
        public string DistanceNearestStorm { get; set; }
        
        public string RecentRain { get; set; }
        public string Cloudiness { get; set; }

        public List<Forecast> Forecasts { get; set; }

        public Conditions()
        {
            City = DayOfWeek = Condition = Temperature = Humidity = Wind = WindDesc = string.Empty;
            WindDirection = WindChill = High = Low = Sunrise = Sunset = Precipitation = MoonIllumintion = string.Empty;
            RecentRain = Cloudiness = string.Empty;

            Forecasts = new List<Forecast>();
        }

        //public void SetWindDirection(string dir)
        //{
        //    string match = DirectionMappings.Keys.FirstOrDefault(key => dir.Contains(key));

        //    if(match != null)
        //    {
        //        WindDirection = DirectionMappings[match];
        //    }
        //}
    }
}
