using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Kinect;
using Ruby.Mind;
using System.Speech.AudioFormat;
using Swiss;
using System.Speech.Recognition;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Ruby.Internal;
using System.Threading.Tasks;
using System.Threading;

namespace Ruby.Mind
{
    public class SpeechRecognizer
    {
        private const double MINIMUM_CONFIDENCE = .7;

        private SpeechRecognitionEngine kinectEngine;
        private KinectSensor Kinect;

        private KinectAudioSource AudioSource;

        bool ready;

        private bool listening, speaking, sleeping;

        public SpeechRecognizer()
        {
            try
            {
                kinectEngine = new SpeechRecognitionEngine();

                listening = false;
                sleeping = false;
                speaking = false;

                ready = true;
            }
            catch (Exception e)
            {
                ready = false;
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };

            var recogs = SpeechRecognitionEngine.InstalledRecognizers();

            return SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault();
        }

        public void Initialize()
        {
            if (!ready) { return; }

            if (!Core.Internal.isDreaming)
            {
                DiscoverSensor();

                if (Kinect != null)
                {
                    InitializeKinectSensor();
                    InitializeKinectEngine();
                    Task.Factory.StartNew(() => Start());
                }
            }
        }

        private void InitializeKinectSensor()
        {
            try
            {
                Kinect.Start();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void DiscoverSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    Kinect = sensor;
                    break;
                }
            }
        }

        private void InitializeKinectEngine()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            Grammar gram = GenerateGrammar();

            if (ri == null)
            {
                return;
            }

            try
            {
                kinectEngine = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                return;
            }

            kinectEngine.LoadGrammar(gram);
            kinectEngine.SpeechRecognized += this.SpeechRecognized;
        }

        public Grammar GenerateGrammar()
        {
            Choices words = new Choices();
            words.Add("Hello Ruby");
            words.Add("Ruby weather");

            List<string> keys = new List<string>() { "Hello Ruby", "silent", "sleep", "yes", "desktop" };
            keys.ForEach((key) => words.Add(key));

            var targetCommands = Core.Memory.Commands.Where(cm => cm.Text.StartsWithIgnoreCase("play some")).ToList();

            foreach(var command in Core.Memory.Commands)
            {
                string formattedCommand = command.Text.TrimCharactersWithin(new char[] { '"', '\'' });
                formattedCommand = "ruby " + formattedCommand;

                words.Add(formattedCommand);
            }

            GrammarBuilder build = new GrammarBuilder();
            build.Append(words);

            Grammar gram = new Grammar(build);

            return gram;
        }

        private void Start()
        {
            AudioSource = Kinect.AudioSource;
            AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = AudioSource.Start();

            SpeechAudioFormatInfo inf = new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null);
            kinectEngine.SetInputToAudioStream(kinectStream, inf);
            kinectEngine.RecognizeAsync(RecognizeMode.Multiple);

            Kinect.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
            Kinect.AudioSource.AutomaticGainControlEnabled = false;
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
        private void Execute_Command(string comm, double confidence)
        {
            if (comm.ContainsIgnoreCase("hello Ruby"))
            {
                if (!listening)
                {
                    listening = true;
                    Brain.Language.Speak("Hello sir", Core.Internal.isQuiet);
                }
            }
            else if (listening)
            {
                if (comm.Equals("Ruby you there"))
                {
                    SpeechUtility.SpeakText("Here sir", Core.Internal.isQuiet);
                }
                else
                {
                    Brain.Process(comm);
                    kinectEngine.RequestRecognizerUpdate();
                }
            }

            Console.WriteLine(comm);

            kinectEngine.RequestRecognizerUpdate();
        }

        public void DisableRecognition()
        {
            kinectEngine.SpeechRecognized -= SpeechRecognized;
        }

        public void EnableRecognition()
        {
            kinectEngine.SpeechRecognized += SpeechRecognized;
            kinectEngine.SpeechHypothesized += KinectEngine_SpeechHypothesized;
        }

        private void KinectEngine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Brain.Language.Speak("I am hypothesizing that you said " + e.Result.ToString(), false);
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            speaking = true;

            double confidence = e.Result.Confidence;
            string text = e.Result.Text;

            if(!text.EqualsIgnoreCase("hello Ruby"))
            {
                if (text.Length < 6 || !text.StartsWithIgnoreCase("Ruby"))
                {
                    //kinectEngine.RequestRecognizerUpdate();
                    return;
                }

                text = text.Substring("Ruby".Length).Trim();
            }

            if (confidence >= MINIMUM_CONFIDENCE)
            {
                Execute_Command(text, confidence);
                Console.WriteLine(text);
            }
            else
            {
                string conf = Math.Round(confidence * 100, 2).ToString();
                string output = string.Format("I am {0} percent sure you are saying {1}, but not sure enough", conf, text);
            }
        }
    }
}
