using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Internal
{
    public class TimeCortex
    {
        public DateTime Date { get; set; }

        public string Tomorrow { get; set; }

        public string Day { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }

        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        public TimeCortex()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            Date = DateTime.Now;

            Day = Date.DayOfWeek.ToString();
            Tomorrow = Date.AddDays(1).DayOfWeek.ToString();
            Month = Date.ToString("MMMM");
            Year = Date.Year;

            Hour = Date.Hour;
            Minute = Date.Minute;
            Second = Date.Second;
        }
    }
}
