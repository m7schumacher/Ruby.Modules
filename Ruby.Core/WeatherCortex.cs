using Newtonsoft.Json.Linq;
using Swiss;
using Swiss.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Atlas.Internal
{
    public class WeatherCortex
    {
        public bool HasMadeRequest { get; set; }
        private Timer _updater;

        public List<Forecast> Forecasts { get; set; }
        public Forecast Today { get { return Forecasts.First(fr => fr.Period.Equals(Core.Time.Day)); } }
        public Forecast Tomorrow { get { return Forecasts.First(fr => fr.Period.Equals(Core.Time.Tomorrow)); } }

        public Conditions CurrentConditions { get; set; }

        public WeatherCortex()
        {
            InitializeValues();
            HasMadeRequest = false;
        }

        public void InitializeValues()
        {
            Task tsk = Task.Factory.StartNew(() => 
            {
                while(String.IsNullOrEmpty(Core.Location.Address)) { }

                _updater = new Timer(600000);
                _updater.Elapsed += new ElapsedEventHandler(UpdateWeather);
                _updater.Enabled = true; 
            });            
        }

        public void UpdateWeather(object sender, ElapsedEventArgs e)
        {
            WeatherAPI _weather = new WeatherAPI(Core.FilePaths.WeatherKey);

            var result = _weather.GetCurrentWeather(Core.Location.Latitude, Core.Location.Longitude);

            CurrentConditions = new Conditions()
            {
                Cloudiness = result.CloudCover.ToString(),
                Temperature = result.Temperature.ToString(),
                Wind = result.WindSpeed.ToString(),
                WindDirection = result.WindDirection.ToString(),
                Humidity = result.Humidity.ToString(),
                High = result.Today.High.ToString(),
                Low = result.Today.Low.ToString(),
                Precipitation = result.ChanceOfPrecip.ToString(),
                Sunset = result.Today.Sunset.ToShortTimeString(),
                Sunrise = result.Today.Sunrise.ToShortTimeString(),
                DistanceNearestStorm = result.DistanceNearestStorm.ToString(),
                DirectionNearestStorm = result.DirectionNearestStorm.ToString(),
                FeelsLike = result.FeelsLike.ToString(),
                DewPoint = result.DewPoint.ToString(),
                Condition = result.Summary
            };

            Forecasts = result.Forecasts.Select(fr => new Forecast(fr)).ToList();

            var stop = 0;

            //string core_url = "http://api.wunderground.com/api/";

            //string conditions_url = String.Format("{0}{1}/{2}/q/{3}/{4}.json",
            //    core_url, Brain.FilePaths.WeatherKey, "conditions", Brain.Location.State, Brain.Location.City);

            //string astro_url = String.Format("{0}{1}/{2}/q/{3}/{4}.json",
            //    core_url, Brain.FilePaths.WeatherKey, "astronomy", Brain.Location.State, Brain.Location.City);

            //string forecast_url = String.Format("{0}{1}/{2}/q/{3}/{4}.json",
            //    core_url, Brain.FilePaths.WeatherKey, "forecast", Brain.Location.State, Brain.Location.City);

            //string conditions_xml = InternetUtility.MakeWebRequest(conditions_url, 10000);
            //string astro_xml = InternetUtility.MakeWebRequest(astro_url, 1000);
            //string forecast_xml = InternetUtility.MakeWebRequest(forecast_url, 1000);

            //JObject conditions = JObject.Parse(conditions_xml);
            //JObject astronomy = JObject.Parse(astro_xml);
            //JObject forecast = JObject.Parse(forecast_xml);

            //if(conditions != null && astronomy != null && forecast != null)
            //{
            //    Conditions currentConditions = GatherCurrentConditions(conditions, astronomy, forecast);

            //    Forecast tonight = currentConditions.Forecasts.FirstOrDefault(fore => fore.Period.ToLower().Equals(Brain.Time.Day.ToLower() + " night"));
            //    Forecast tomorrow = currentConditions.Forecasts.FirstOrDefault(fore => fore.Period.ToLower().Equals(Brain.Time.Tomorrow.ToLower()));
            //    Forecast tomorrow_night = currentConditions.Forecasts.FirstOrDefault(fore => fore.Period.ToLower().Equals(Brain.Time.Tomorrow.ToLower() + " night"));

            //    CurrentConditions = currentConditions;

            //    Tonight = tonight;
            //    Tomorrow = tomorrow;
            //    Tomorrow_Night = tomorrow_night;

            //    if (!HasMadeRequest)
            //    {
            //        HasMadeRequest = true;
            //    }
            //}  
            //else
            //{
            //    CurrentConditions = null;
            //}
        }

        private Conditions GatherCurrentConditions(JObject weather, JObject astro, JObject forecast)
        {
            var cond_main = weather["current_observation"];
            var astro_main = astro["moon_phase"];
            var forecast_main = forecast["forecast"];

            Conditions curr = new Conditions();
            curr.Temperature = (string)cond_main["temp_f"];
            curr.Humidity = (string)cond_main["relative_humidity"];
            curr.Wind = (string)cond_main["wind_mph"];
            curr.WindDesc = (string)cond_main["wind_string"];
            curr.WindDirection = DetermineDirection((string)cond_main["wind_degree"]);
            curr.WindChill = (string)cond_main["windchill_f"];
            curr.Condition = (string)cond_main["weather"];
            curr.Precipitation = (string)cond_main["precip_today_in"];
            curr.MoonIllumintion = (string)astro_main["percentIlluminated"];
            curr.Sunrise = (string)astro_main["sunrise"]["hour"] + ":" + (string)astro_main["sunrise"]["minute"];
            curr.Sunset = (string)astro_main["sunset"]["hour"] + ":" + (string)astro_main["sunset"]["minute"];

            foreach (var day in forecast_main["txt_forecast"]["forecastday"].Children())
            {
                string description = (string)day["fcttext"];
                string period = (string)day["title"];

                if (period.Contains(Core.Time.Tomorrow) || period.Contains(Core.Time.Day))
                {
                    Forecast fore = new Forecast(period, description);
                    curr.Forecasts.Add(fore);
                }
            }

            return curr;
        }

        private string DetermineDirection(string deg)
        {
            int degree = Convert.ToInt16(deg);
            var val = (int)((degree / 22.5) + .5);

            string[] directions = new string[]
            { 
                "North","North North East",
                "North East", "East North East",
                "East","East South East", "South East",
                "South South East","South","South South West",
                "South West","West South West","West","West North West",
                "North West","North North West" 
            };

            return directions[(val % 16)];
        }
    }
}
