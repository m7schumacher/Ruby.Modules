using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;
using Ruby.Internal;

namespace Ruby.Movements
{
    internal class Configurer : Reflex
    {
        Dictionary<string[], string> actions;

        string[] quiet, learn;

        public Configurer() : base()
        {
            Name = "Settings";
            ID = "170";

            quiet = new string[] { "go dark", "quiet", "speak" };
            learn = new string[] { "start learning", "stop learning" };
        }

        public override void GenerateRecognizedPhrases()
        {
            RecognizedPhrases = new Dictionary<string, string[]>()
            {
                { "go {}", new string[] { "dark", "quiet", "silent" } },
                { "start {}", new string[] { "learning", "listening" } },
            };
        }

        public override void Initialize()
        {
            actions = new Dictionary<string[], string>()
            {
                { quiet, "ChangeQuietMode" },
                { learn, "ChangeLearningMode" }
            };
        }

        public override string Execute()
        {
            string input = Core.External.UserInput;
            string response = Error();

            string[] winner = actions.Keys.FirstOrDefault(key => key.Any(k => input.Contains(k)));

            if(winner != null)
            {
                string method = actions[winner];

                Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod(method);

                var methods = thisType.GetMethods();
                response = theMethod.Invoke(this, null).ToString();
            }

            return response;
        }

        public string ChangeQuietMode()
        {
            string input = Core.External.UserInput;
            bool isQuiet = Core.Internal.isQuiet;
            string output = string.Empty;

            string[] turnOffs = new string[] { "off", "speak" };
            string[] turnOns = new string[]{ "on", "go quiet", "go dark", "silent", "silence" };

            if(turnOffs.Any(str => input.Contains(str)))
            {
                output = Core.Internal.isQuiet ? "I am now in speaking mode sir" : "I am already in speaking mode";
                Core.Internal.isQuiet = false;
            }
            else if(turnOns.Any(str => input.Contains(str)))
            {
                output = Core.Internal.isQuiet ? "I am already in silent mode" : "Going silent sir";
                Core.Internal.isQuiet = true;
            }
            else
            {
                return Error();
            }

            return output;
        }

        public string ChangeLearningMode()
        {
            string input = Core.External.UserInput;
            bool isQuiet = Core.Internal.isQuiet;
            string output = string.Empty;

            if(input.Contains("stop learning"))
            {
                output = Core.Internal.isLearning ? "Stopping learning sir" : "I am not learning";
                Core.Internal.isLearning = false;
            }
            else if(input.Contains("start learning"))
            {
                output = Core.Internal.isLearning ? "I am already learning sir" : "I am now learning sir";
                Core.Internal.isLearning = true;
            }
            else
            {
                return Error();
            }

            return output;
        }

        private string Error()
        {
            return "Unable to change setting";
        }
    }
}
