using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace Atlas.Internal
{
    public class LocationCortex
    {
        private string MAP_URL = "http://maps.google.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";

        public Dictionary<string, string> StoredAddresses { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        GeoCoordinateWatcher watcher;

        Timer _updater;

        public LocationCortex()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            watcher = new GeoCoordinateWatcher();

            StoredAddresses = new Dictionary<string,string>()
            {
                { "home", "4822 Meadow Creek Drive, Fargo ND, 58104" },
                { "apartment", "1900 Dakota Drive North, Fargo ND, 58102" },
                { "work", "501 4th Street North, Fargo ND, 58102" }
            };

            Address = string.Empty;
            Latitude = 0;
            Longitude = 0;

            Task tsk = Task.Factory.StartNew(() =>
            {
                UpdateLocation(null, null);

                _updater = new Timer(600000);
                _updater.Elapsed += new ElapsedEventHandler(UpdateLocation);
                _updater.Enabled = true;
            });   
        }

        public void UpdateLocation(object sender, ElapsedEventArgs args)
        {
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(GeoPositionChanged);
            watcher.Start();
        }

        private void GeoPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Core.Location.Latitude = e.Position.Location.Latitude;
            Core.Location.Longitude = e.Position.Location.Longitude;

            MAP_URL = string.Format(MAP_URL, Core.Location.Latitude, Core.Location.Longitude);

            XElement sitemap = XElement.Load(MAP_URL);
            XElement results = sitemap.Elements().First(elem => elem.Name.LocalName.Equals("result"));
            XElement formatted_address = results.Elements().First(elem => elem.Name.LocalName.Equals("formatted_address"));

            SetAddress(formatted_address.Value);

            watcher.Stop();
        }

        public void SetAddress(string address)
        {
            string[] bits = address.Split(',');

            Street = bits[0];
            City = bits[1].Trim();
            State = bits[2].Split(' ')[1];
            ZipCode = bits[2].Split(' ')[2];
            Country = bits[3].Trim();

            Address = address;
        }
    }
}
