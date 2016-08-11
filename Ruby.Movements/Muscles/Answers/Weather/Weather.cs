using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Swiss;
using Ruby.Internal;

namespace Ruby.Muscle
{
    internal class Weather : Answer
    {
        public Weather() : base()
        {
            Name = "Weather";
            Key = "weather";
            ID = "75";

            DefaultSpec = "today";
        }

        public override void EstablishRecognizers()
        {
            Recognizers = new Dictionary<string, string[]>()
            {
                { "what is the {} outside", new string[] { "temp", "temperature", "wind", "humidity", "weather", "windchill" } },
                { "how {} is it", new string[] { "cold", "warm", "hot", "windy", "humid", "rainy" } },
                { "|| weather look {}", new string[] { "today", "tomorrow", "tomorrow night" } },
                { "is it {} outside", new string[] { "raining", "cloudy", "sunny", "stormy", "rain", "snow" } }
            };
        }

        public override void SetPrimarySpecs()
        {
            string[] forecasts = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Tomorrow", "Today" };

            PrimarySpecs = new Dictionary<string, string[]>()
            {
                { "fore", forecasts },
            };
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>()
            {
                { "high", new string[] { "how hot will", "high", "high temperature" } },
                { "low", new string[] { "how cold will", "low", "low temperature" } },
                { "rise", new string[] { "sunrise", "dawn", "sun rise" } },
                { "set", new string[] { "sunset", "dusk", "sun rise", "dark out" } },
                { "humid", new string[] { "humid", "humidity" } },
                { "wind", new string[] { "windy", "how windy", "wind" } },
                { "temp", new string[] { "how hot", "how cold", "temperature", "the temp", "warm", "cold" } },
                { "storm", new string[] { "nearest storm", "rain nearby", "nearest rain", "nearest storm" } },
                { "precip", new string[] { "storm", "chances of rain", "chance of precip", "will it rain", "will it snow", "rain", "snow", "storm" } },
                { "cloudiness", new string[] { "how cloudy", "cloud cover" } },
                { "dewpoint", new string[] { "dewpoint", } },
                { "visibility", new string[] { "visibility", "how far can you see" } },
                { "outlook", new string[] { "outlook", "today's", "going", "look", "forecast", "today", "look like", "be like" } }
            };
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = "today";
        }

        public override void GatherValues()
        {
            string input = Core.External.UserInput;

            if (input.Equals("weather"))
            {
                Spec = "overview";
            }

            if(!Core.Weather.HasMadeRequest)
            {
                Core.Weather.UpdateWeather(null, null);
            }

            Conditions cond = Core.Weather.CurrentConditions;
            List<Forecast> forecasts = Core.Weather.Forecasts;

            var targetForecast = forecasts.FirstOrDefault(fore => input.ContainsIgnoreCase(fore.Period)) ?? Core.Weather.Today;

            if(cond == null)
            {
                Values = null;
            }
            else
            {
                Values = new Dictionary<string, string>()
                {
                    { "fore_day", targetForecast.Period },
                    { "fore_wind_speed", targetForecast.WindSpeed },
                    { "fore_wind_direction", targetForecast.WindDirection },
                    { "fore_precip", targetForecast.OutputPrecip() },
                    { "fore_cloudiness", targetForecast.Cloudiness },
                    { "fore_sunrise", targetForecast.Sunrise },
                    { "fore_sunset", targetForecast.Sunset },
                    { "fore_cond", targetForecast.Condition },
                    { "fore_high", cond.High },
                    { "fore_low", cond.Low },
                    { "fore_humid", cond.Humidity },
                    { "temp", cond.Temperature },
                    { "cond", cond.Condition },
                    { "wind", cond.Wind },
                    { "wind_direction", cond.WindDirection },
                    { "humid", cond.Humidity },
                    { "dewpoint", cond.DewPoint },
                    { "visibility", cond.Visibility },
                    { "storm_distance", cond.DistanceNearestStorm },
                    { "storm_direction", cond.DirectionNearestStorm },
                    { "cloudiness", cond.Cloudiness },
                    { "precip", cond.Precipitation }
                };
            }
        }
    }
}
