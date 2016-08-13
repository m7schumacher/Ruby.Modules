using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Calendar.v3.Data;
using Ruby.Internal;

namespace Ruby.Movements
{
    internal class Calendar : Answer
    {
        CalendarService service;
        string CLIENT_ID;
        string CLIENT_SECRET;
        string Username;

        int numberOfEventsToday;
        string timeDayStarts;

        public Calendar() : base()
        {
            Name = "Calendar";
            Key = "calendar";

            ID = "76";

            CLIENT_ID = Core.FilePaths.GoogleAPIClient;
            CLIENT_SECRET = Core.FilePaths.GoogleAPISecret;
            Username = Core.FilePaths.GoogleUsername;
        }

        public override void Initialize()
        {
            string[] scopes = new string[] { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarReadonly };

            //try
            //{
            //    UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync
            //    (
            //        new ClientSecrets { ClientId = CLIENT_ID, ClientSecret = CLIENT_SECRET },
            //        scopes,
            //        Username,
            //        CancellationToken.None,
            //        new FileDataStore("Daimto.GoogleCalendar.Auth.Store")
            //    ).Result;

            //    service = new CalendarService(new BaseClientService.Initializer()
            //    {
            //        HttpClientInitializer = credential,
            //        ApplicationName = "Calendar API Sample",
            //    });
            //}
            //catch(Exception trouble)
            //{
            //    Disable();
            //    return;
            //}
        }

        public override void GenerateRecognizedPhrases()
        {
            RecognizedPhrases = new Dictionary<string, string[]>()
            {
                { "what {} is it", new string[] { "day", "time", "month", "year" } },
                { "what is the {}", new string[] { "day", "time", "month", "year" } },
                { "brady wants to know what {} it is", new string[] { "day", "time", "month", "year" } }
            };
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>()
            {
                { "time", new string[] { "what time is it" } },
                { "year", new string[] { "year is it", "the year" } },
                { "month", new string[] { "month is it", "the month" } },
                { "shots", new string[]{ "brady wants to know", "tell brady" } },
                { "date", new string[] { "date" } },
                { "day", new string[] { "what day" } },
                { "first", new string[] { "first", "start" } },
                { "outlook", new string[] { "my day", "calendar", "schedule" } },
            };
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = "time";
        }

        public override void GatherValues()
        {
            //List<CalendarEvent> events = GetTodaysEvents().ToList();
            //CalendarEvent first = null;

            //timeDayStarts = string.Empty;
            //numberOfEventsToday = events.Count;

            //string outlookResponse = string.Empty;

            //if (numberOfEventsToday > 0)
            //{
            //    first = events.ElementAt(0);
            //    timeDayStarts = Convert.ToDateTime(first.StartTime).ToShortTimeString();
            //}

            //if(numberOfEventsToday == 0)
            //{
            //    outlookResponse = "no scheduled events today";
            //}
            //else if(numberOfEventsToday == 1)
            //{
            //    outlookResponse = "only one event today, it starts at" + timeDayStarts;
            //}
            //else
            //{
            //    outlookResponse = numberOfEventsToday + " events today, the first starts at " + timeDayStarts;
            //}

            Values = new Dictionary<string, string>()
            {
                { "date", DateTime.Now.ToShortDateString() },
                { "day", DateTime.Today.DayOfWeek.ToString() },
                { "year", Core.Time.Year.ToString() },
                { "month", Core.Time.Month },
                { "one", "one" },
                { "multi", numberOfEventsToday.ToString() },
                { "time", DateTime.Now.ToShortTimeString() },
                { "first", timeDayStarts }
                //{ "outlook", outlookResponse }
            };
        }

        public CalendarEvent CreateCalendarEvent(Event eventdata)
        {
            CalendarEvent item = new CalendarEvent
            {
                Id = eventdata.Id,
                Title = eventdata.Summary,
                Description = eventdata.Description,
                Location = eventdata.Location,
                IsBusy = (eventdata.Transparency != "transparent") ? true : false
            };

            EventDateTime start = eventdata.Start;
            item.StartDate = Convert.ToString(start.DateTime);
            item.StartTime = Convert.ToDateTime(start.DateTime).ToString("HH:mm");
            EventDateTime end = eventdata.End;
            item.EndDate = Convert.ToString(end.DateTime);
            item.EndTime = Convert.ToDateTime(end.DateTime).ToString("HH:mm");

            return item;
        }

        public IEnumerable<CalendarEvent> GetTodaysEvents()
        {
            IList<CalendarListEntry> list = service.CalendarList.List().Execute().Items;
            List<CalendarEvent> calendarEvents = new List<CalendarEvent>();

            DateTime startDate = DateTime.Today;

            EventsResource.ListRequest req = service.Events.List(list[0].Id);
            req.TimeMin = Convert.ToDateTime(startDate);
            req.TimeMax = Convert.ToDateTime(startDate).AddDays(1);
            req.SingleEvents = true;
            req.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = req.Execute();

            foreach (Event eventdata in events.Items)
            {
                CalendarEvent item = CreateCalendarEvent(eventdata);
                calendarEvents.Add(item);
            }

            return calendarEvents;
        }
    }
}
