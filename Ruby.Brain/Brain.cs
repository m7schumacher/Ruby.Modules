﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;
using System.Runtime.InteropServices;
using Ruby.Internal;
using Swiss;
using Ruby.Movements;

namespace Ruby.Mind
{
    public class Brain
    {
        public static LanguageCortex Language { get; set; }
        public static SensoryCortex Senses { get; set; }
        public static MotorCortex Motor { get; set; }

        public static List<Lobe> Lobes { get; set; }

        public static void Awaken(bool local)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Senses = new SensoryCortex();
            Motor = new MotorCortex();
            Language = new LanguageCortex();

            Core.Internal.isLocal = local;
            Core.Internal.isQuiet = false;

            bool dreaming = false;

            if (dreaming || Core.Internal.isDreaming)
            {
                Dream();
                MuscleGate.Terminate();
            }

            while(Core.Internal.State == Hypothalmus.States.Cold) { }

            Senses.Immune.Initialize(true);

            Motor.GatherCommands();

            Console.WriteLine("Listening!");

            watch.Stop();

            Console.WriteLine("Initialization took {0} seconds", watch.Elapsed.TotalSeconds.Round(2));
            Console.WriteLine("\n\n\nRuby is now ready:\n\n\n");

            Core.Internal.State = Hypothalmus.States.Ready;
        }

        private static void GatherCommands()
        {
            if (Core.Internal.IsConnectedToSpotify)
            {
                foreach (string song in MotorCortex.Muscle.GetSongsFromSpotify())
                {
                    Core.Memory.Commands.Add(new Command() { Text = "play " + song.TrimPunctuation(), ID = "151" });
                }
            }
        }

        public static void FireUp()
        {
            Core.Initialize();
        }

        public static void Dream()
        {

        }

        public static void Process(string input)
        {
            string response = string.Empty;

            try
            {
                input = input.StartsWith("ruby") ? input.Remove("ruby").Trim() : input.Trim();

                var key = Senses.Process(input);
                response = Motor.React(key);
            }
            catch(Exception e)
            {
                response = "I encountered an error sir";
            }
            
            Language.Speak(response, Core.Internal.isQuiet);
        }
    }
}
