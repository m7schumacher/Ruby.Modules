using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ruby.Internal
{
    public class Forecast
    {
        private Dictionary<string, string> DirectionMappings = new Dictionary<string, string>()
        {
            { " S", " from the south " },
            { " N", " from the north " },
            { " W", " from the west " },
            { " E", " from the east " },
        };

        public string Period { get; set; }

        public string High { get; set; }
        public string Low { get; set; }

        public string WindSpeed { get; set; }
        public string WindDirection { get; set; }

        public string Cloudiness { get; set; }
        public string ChanceOfPrecipitation { get; set; }
        public string TypeOfPrecipitation { get; set; }

        public string Condition { get; set; }
        public string Humidity { get; set; }

        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        public Forecast(Swiss.API.Forecast fore)
        {
            Period = fore.DayOfWeek;
            High = fore.High.ToString();
            Low = fore.Low.ToString();
            WindSpeed = fore.WindSpeed.ToString();
            WindDirection = fore.WindDirection.ToString();
            Cloudiness = fore.Cloudiness.ToString();
            ChanceOfPrecipitation = fore.ChanceOfPrecip.ToString();
            TypeOfPrecipitation = fore.TypeOfPrecip.ToString();
            Condition = fore.Summary;

            Sunrise = fore.Sunrise.ToShortTimeString();
            Sunset = fore.Sunset.ToShortTimeString();

            Humidity = fore.Humidity.ToString();
        }

        public string OutputPrecip()
        {
            return !ChanceOfPrecipitation.Equals("0") ? "a " + ChanceOfPrecipitation + " percent chance of " + TypeOfPrecipitation
                                                      : "no chance of precipitation";
        }

        public string OutputWind()
        {
            return string.Format("{0} mile per hour winds from the {1}", WindSpeed, WindDirection);
        }

        public Forecast(string period, string description)
        {
            //Period = period;

            //string[] bits = description.Split('.');

            //for (int i = 0; i < bits.Length; i++)
            //{
            //    string str = bits[i].Trim();

            //    if(i == 0)
            //    {
            //        Condition = str;
            //    }
            //    else if(str.StartsWith("High") || str.StartsWith("Low"))
            //    {
            //        string temp = string.Empty;
            //        string nature = string.Empty;

            //        Match mtch = Regex.Match(str, @"\d+F");
            //        Match highlow = Regex.Match(str, "High|Low");

            //        if(mtch.Success)
            //        {
            //            temp = mtch.Value;
            //        }

            //        if(highlow.Success)
            //        {
            //            nature = highlow.Value;
            //        }

            //        Temperature = nature + " of " + temp.Replace("F", " degrees");
            //    }
            //    else if(str.StartsWith("Winds"))
            //    {
            //        string match = DirectionMappings.Keys.FirstOrDefault(key => str.Contains(key));

            //        if(match != null)
            //        {
            //            Wind = str.Replace(match, DirectionMappings[match]);
            //        }
            //        else { Wind = str; }

            //        Wind = Regex.Replace(Wind, "Winds|winds", "wind");
            //    }
            //    else if(str.StartsWith("Chance of"))
            //    {
            //        List<string> microbits = str.Split(' ').ToList();
            //        microbits.Add(microbits.Last());

            //        string[] revive = microbits.ToArray();
            //        revive[revive.Length - 2] = "is";

            //        string output = string.Empty;

            //        foreach(string bit in revive)
            //        {
            //            output += bit + " ";
            //        }

            //        Precipitation = output.Trim();
            //    }
            //}
        }

        public override string ToString()
        {
            return Period;
        }
    }
}
