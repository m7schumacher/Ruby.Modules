using Ruby.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Muscle{
    internal class SmallTalk : Reflex
    {
        string purpose;
        string identity;
        string capability;

        public SmallTalk() : base()
        {
            Name = "SmallTalk";
            ID = "168";

            purpose = "I am designed to help with everything from information retrieval to every day life";
            identity = "My name is Ruby and I am a personal assistant.";
            capability = "I can tell you the weather, give location and directions, tell you sports scores and news, " +
                "even give an update on your fitbit statistics for the day, and much more";
        }

        public override string Execute()
        {
            string input = Core.External.UserInput;

            string[] who = { "who are you", "what is Ruby", "what are you" };
            string[] what = { "what can you do", "what are you cabale of", "can you do" };
            string[] why = { "what do you do" };

            string output = string.Empty;

            if (who.Any(str => input.Contains(str)))
            {
                output = identity;
            }
            else if(what.Any(str => input.Contains(str)))
            {
                output = capability;
            }
            else if(why.Any(str => input.Contains(str)))
            {
                output = purpose;
            }

            return output;
        }
    }
}
