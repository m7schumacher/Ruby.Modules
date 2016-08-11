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

namespace Ruby.Mind
{
    public class SpeechRecognizer
    {
        private SpeechRecognitionEngine kinectEngine;
        private KinectSensor Kinect;

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
                    Start();
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

            List<string> keys = new List<string>() { "Hello Ruby", "Ruby", "silent", "sleep", "yes", "desktop" };
            keys.ForEach((key) => words.Add(key));

            Core.Memory.Commands.ForEach((cm) => words.Add(cm.Text.TrimCharactersWithin(new char[] { '"', '\'' })));

            GrammarBuilder build = new GrammarBuilder();
            build.Append(words);

            Grammar gram = new Grammar(build);

            return gram;
        }

        private void Start()
        {
            var audioSource = Kinect.AudioSource;
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = audioSource.Start();

            SpeechAudioFormatInfo inf = new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null);
            kinectEngine.SetInputToAudioStream(kinectStream, inf);
            kinectEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
        private void Execute_Command(string comm, double confidence)
        {
            if (comm.ToLower().Contains("hello Ruby"))
            {
                if (!listening)
                {
                    listening = true;
                    SpeechUtility.SpeakText("Hello sir", Core.Internal.isQuiet);
                }

                kinectEngine.RequestRecognizerUpdate();
            }
            else if (listening)
            {
                if (comm.Equals("Ruby you there"))
                {
                    SpeechUtility.SpeakText("Here sir", Core.Internal.isQuiet);
                }
                else if (comm.Equals("silent"))
                {
                    listening = false;
                    SpeechUtility.SpeakText("Going silent", Core.Internal.isQuiet);
                }
                else if (comm.Equals("yes"))
                {
                    if (sleeping) { SetSuspendState(false, true, true); }
                }
                else
                {
                    Brain.Process(comm);
                    kinectEngine.RequestRecognizerUpdate();
                }
            }
        }

        public void DisableRecognition()
        {
            kinectEngine.SpeechRecognized -= SpeechRecognized;
        }

        public void EnableRecognition()
        {
            kinectEngine.SpeechRecognized += SpeechRecognized;
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            double confidence = e.Result.Confidence;
            string text = e.Result.Text;

            if(!text.EqualsIgnoreCase("hello Ruby"))
            {
                if (text.Length < 6 || !text.StartsWith("Ruby"))
                {
                    kinectEngine.RequestRecognizerUpdate();
                    return;
                }

                text = text.Substring(6);
            }

            if (confidence >= .65)
            {
                Execute_Command(text, confidence);
            }
            else
            {
                string conf = Math.Round(confidence * 100, 2).ToString();
                Swiss.SpeechUtility.SpeakText("I hear " + text + " at " + conf + " confidence", false);
            }
        }
    }
}
