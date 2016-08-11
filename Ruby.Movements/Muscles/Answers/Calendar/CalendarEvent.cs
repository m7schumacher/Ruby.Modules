using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby{
    internal class CalendarEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsBusy { get; set; }

        public string toString()
        {
            return Title + " [ " + StartTime.ToString() + " - " + EndTime.ToString() + " ]";
        }
    }
}
