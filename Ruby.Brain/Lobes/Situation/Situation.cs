using Ruby.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ruby.Mind
{
    public enum Moods { Good, Bad, Moderate, Default, None };
    public enum Condition { None, Rainy, Snowy, Cloudy, Sunny };
    public enum Time { early_morning, morning, noonish, afternoon, evening, night, late_night, None };
    public enum Temp { subzero, freezing, cold, cool, mild, warm, hot, scalding };

    public class Situation
    {
        public Moods Mood { get; set; }
        public Condition Weather { get; set; }
        public Time Time { get; set; }
        public Temp Temp { get; set; }
        public Condition Condition { get; set; }

        public string Topic { get; set; }

        public Timer _updater;

        public Situation()
        {
            Mood = Moods.Default;
            Topic = string.Empty;

            Task tsk = Task.Factory.StartNew(() =>
            {
                while (String.IsNullOrEmpty(Core.Location.Address)) { }

                UpdateSituation(null, null);

                _updater = new Timer(1800000);
                _updater.Elapsed += new ElapsedEventHandler(UpdateSituation);
                _updater.Enabled = true;
            });          
        }

        private void UpdateSituation(object sender, ElapsedEventArgs e)
        {
            AnalyzeWeather();
            AnalyzeTime();
        }

        private void AnalyzeTime()
        {
            int time = DateTime.Now.Hour;

            if(time <= 8){ Time = Time.early_morning; }
            else if(time > 8 && time <= 11){ Time = Time.morning; }
            else if(time > 11 && time <= 13){ Time = Time.noonish; }
            else if(time > 13 && time <= 17){ Time = Time.afternoon; }
            else if(time > 17 && time <= 20){ Time = Time.evening; }
            else if(time > 20 && time <= 23){ Time = Time.night; }
            else { Time = Time.late_night; }
        }

        private void AnalyzeWeather()
        {
            if (Core.Weather.HasMadeRequest)
            {
                Conditions current_conditions = Core.Weather.CurrentConditions;

                int temperature = Convert.ToInt16(current_conditions.Temperature);

                if (temperature >= 85) { Temp = Temp.hot; }
                else if (temperature < 85 && temperature >= 70) { Temp = Temp.warm; }
                else if (temperature < 70 && temperature >= 50) { Temp = Temp.mild; }
                else if (temperature < 50 && temperature >= 32) { Temp = Temp.cool; }
                else if (temperature < 32 && temperature >= 20) { Temp = Temp.cold; }
                else if (temperature < 20 && temperature > 0) { Temp = Temp.freezing; }
                else { Temp = Temp.subzero; }

                string conditions = current_conditions.Condition;

                string[] snow = new string[] { "snow", "flurries" };
                string[] rain = new string[] { "showers", "rain", "storm" };
                string[] clouds = new string[] { "mostly cloudy", "cloudy", "considerable cloudiness" };
                string[] sunny = new string[] { "sunny", "sunshine", "clear" };

                if (snow.Any(word => conditions.Contains(word))) { Condition = Condition.Snowy; }
                else if (rain.Any(word => conditions.Contains(word))) { Condition = Condition.Rainy; }
                else if (clouds.Any(word => conditions.Contains(word))) { Condition = Condition.Cloudy; }
                else if (sunny.Any(word => conditions.Contains(word))) { Condition = Condition.Sunny; }
                else { Condition = Condition.None; }
            }
        }




    }
}
