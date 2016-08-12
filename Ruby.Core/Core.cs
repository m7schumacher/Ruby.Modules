using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Internal
{
    public class Core
    {
        public static LocationCortex Location { get; set; }
        public static WeatherCortex Weather { get; set; }
        public static MachineCortex Machine { get; set; }
        public static TimeCortex Time { get; set; }
        public static MemoryCortex Memory { get; set; }
        public static MusicCortex Music { get; set; }
        public static LanguageCortex Language { get; set; }

        public static Paths FilePaths { get; set; }
        public static Hippocampus Hippo { get; set; }
        public static Hypothalmus Internal { get; set; }
        public static External External { get; set; }

        public static void Initialize()
        {
            Location = new LocationCortex();
            Weather = new WeatherCortex();
            Machine = new MachineCortex();
            Time = new TimeCortex();
            Hippo = new Hippocampus();
            FilePaths = new Paths();
            Internal = new Hypothalmus();
            External = new External();
            Memory = new MemoryCortex();
            Music = new MusicCortex();
            Language = new LanguageCortex();
        }
    }
}
